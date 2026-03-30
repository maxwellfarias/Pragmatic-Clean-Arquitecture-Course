# 09 - Infrastructure: Outbox, Quartz, and Optimistic Concurrency

## Learning Goals
- Understand how Bookify implements the transactional outbox pattern.
- Learn how Quartz schedules background processing.
- Understand optimistic concurrency handling in EF Core.

## Simple Analogy
Imagine writing a letter and logging it in an "outgoing mail" tray:
- Business transaction writes both data and mail entry.
- A mail clerk periodically sends pending letters.

If the clerk is late, the letter is still safe in the tray.

## Concept Explanation
### Transactional Outbox
Bookify writes domain events into `outbox_messages` during `SaveChangesAsync`.
This avoids losing events if the process crashes after DB commit.

### Quartz job processing
`ProcessOutboxMessagesJob`:
- fetches pending messages,
- deserializes domain events,
- publishes via MediatR,
- marks messages as processed (or stores error).

### Optimistic concurrency
`ApartmentConfiguration` adds row version. EF throws `DbUpdateConcurrencyException` when stale writes happen.
Bookify maps it to `ConcurrencyException`.

## Real Code Walkthrough
Source: `src/Bookify.Infrastructure/ApplicationDbContext.cs`

```csharp
public void AddDomainEventsAsOutboxMessages()
{
    var domainEvents = ChangeTracker
       .Entries<IEntity>()
       .Select(entry => entry.Entity)
       .SelectMany(entity => {
           var domEvs = entity.GetDomainEvents();
           entity.ClearDomainEvents(); // Prevent duplicate publication
           return domEvs;
       })
       .Select(domainEvent => new OutboxMessage(
            Guid.NewGuid(),
            _dateTimeProvider.UtcNow,
            domainEvent.GetType().Name,
            JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)
       ))
       .ToList();

    AddRange(domainEvents); // Outbox rows persisted in same transaction
}
```

Source: `src/Bookify.Infrastructure/Outbox/ProcessOutboxMessagesJob.cs`

```csharp
var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

foreach (var outboxMessage in outboxMessages)
{
    Exception? exception = null;

    try
    {
        var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
            outboxMessage.Content,
            JsonSerializerSettings)!;

        await _publisher.Publish(domainEvent, context.CancellationToken); // Publish event
    }
    catch (Exception caughtException)
    {
        exception = caughtException; // Persist error for observability/retry analysis
    }

    await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
}
```

Source: `src/Bookify.Infrastructure/Outbox/ProcessOutboxMessagesJobSetup.cs`

```csharp
options
    .AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobName))
    .AddTrigger(configure =>
        configure
            .ForJob(jobName)
            .WithSimpleSchedule(schedule =>
                schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
```

Source: `src/Bookify.Infrastructure/Configurations/ApartmentConfiguration.cs`

```csharp
builder.Property<uint>("Version").IsRowVersion(); // Optimistic concurrency token
```

## Flow of Execution
1. Domain entity raises event.
2. `SaveChangesAsync` converts event to outbox row.
3. Transaction commits business data + outbox record.
4. Quartz job polls pending outbox rows.
5. Job publishes event and marks row processed.

## Common Pitfalls In This Project
- Using outbox without monitoring failed messages.
- Assuming all aggregates have concurrency tokens (currently focused in one config).
- Serializing events without versioning strategy for long-term evolution.
- Forgetting job interval/batch tuning in production.

## Small Exercises / Checkpoints
1. Find `OutboxOptions` in appsettings and explain each value.
2. Trace one event type from domain to outbox to publisher.
3. Explain why `FOR UPDATE` appears in outbox SQL query.
4. Propose how to handle poison messages (too many failures).
