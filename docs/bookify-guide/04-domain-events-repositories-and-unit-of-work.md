# 04 - Domain Events, Repositories, and Unit of Work

## Learning Goals
- Understand why domain events exist and how Bookify stores them safely.
- Learn repository abstractions in Domain and implementations in Infrastructure.
- Understand Unit of Work orchestration and transaction boundaries.

## Simple Analogy
Picture a restaurant:
- Taking an order is the main business action.
- Ringing the kitchen bell is a **domain event**.
- The waiter doesn’t cook directly; they hand off responsibility.

Events let your domain announce facts without hard-wiring every reaction.

## Concept Explanation
### Domain events
Domain events represent important business facts (e.g., booking reserved).
In Bookify, entities store events internally via base `Entity<TEntityId>`.

### Repositories
Domain defines repository interfaces to abstract persistence details.
Infrastructure implements those interfaces with EF Core.

### Unit of Work
`IUnitOfWork` exposes `SaveChangesAsync` so Application handlers commit business operations as a single unit.

## Real Code Walkthrough
Source: `src/Bookify.Domain/Abstractions/Entity.cs`

```csharp
public abstract class Entity<TEntityId> : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList(); // Exposes read-only copy
    }

    protected void RaiseDomainEvent(IDomainEvent ev)
    {
        _domainEvents.Add(ev); // Entity records facts that happened
    }
}
```

Source: `src/Bookify.Domain/Bookings/IBookingRepository.cs`

```csharp
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(BookingId id, CancellationToken cancellationToken = default);

    Task<bool> IsOverlappingAsync(
        Apartment apartment,
        DateRange duration,
        CancellationToken cancellationToken = default);

    void Add(Booking booking);
}
```

Source: `src/Bookify.Domain/Abstractions/IUnitOfWork.cs`

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default); // Transactional commit point
}
```

Source: `src/Bookify.Infrastructure/ApplicationDbContext.cs`

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    AddDomainEventsAsOutboxMessages(); // Converts in-memory domain events to durable outbox rows
    return await base.SaveChangesAsync(ct);
}
```

## Flow of Execution
1. Domain entity performs behavior and raises domain event.
2. Application handler calls repository `Add(...)` and then `SaveChangesAsync()`.
3. DbContext collects entity domain events.
4. Events are persisted into `outbox_messages` table.
5. Background job later publishes them through MediatR.

## Common Pitfalls In This Project
- Publishing events immediately inside transaction (can cause reliability issues).
- Leaking EF Core APIs into Domain interfaces.
- Forgetting to clear domain events after capturing them for outbox.
- Treating repository as an anemic generic CRUD tool only.

## Small Exercises / Checkpoints
1. Find all domain event types under `src/Bookify.Domain/*/Events`.
2. Trace one event from `RaiseDomainEvent` to outbox row.
3. Explain why repository interfaces live in Domain, not Infrastructure.
4. Add a note about why Unit of Work belongs to an abstraction.
