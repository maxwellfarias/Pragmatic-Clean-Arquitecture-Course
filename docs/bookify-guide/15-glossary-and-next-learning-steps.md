# 15 - Glossary and Next Learning Steps

## Learning Goals
- Consolidate vocabulary used across this guide.
- Connect concepts to concrete Bookify code.
- Build a practical roadmap for your next implementation steps.

## Simple Analogy
A glossary is like a toolbox label board:
- You work faster when each tool has a known name and purpose.
- You avoid mistakes by selecting the right tool for the job.

## Concept Explanation
### Glossary
- **Entity**: Domain object with stable identity over time (`User`, `Booking`).
- **Value Object**: Immutable object defined by value (`Money`, `DateRange`, `Email`).
- **Domain Event**: Fact emitted by domain behavior (`BookingReserverdDomainEvent`).
- **Repository**: Domain abstraction for persistence operations.
- **Unit of Work**: Transaction boundary (`SaveChangesAsync`).
- **Command**: Request that changes state.
- **Query**: Request that reads state.
- **Pipeline Behavior**: Cross-cutting interceptor around handlers.
- **Outbox Pattern**: Durable event storage before asynchronous publication.
- **Optimistic Concurrency**: Detects stale writes using version tokens.
- **Authentication**: Who are you? (JWT identity validation).
- **Authorization**: What can you do? (policy/role/permission checks).

## Real Code Walkthrough
Source: `src/Bookify.Domain/Bookings/Events/BookingReserverdDomainEvent.cs`

```csharp
public record BookingReserverdDomainEvent(BookingId bookingId) : IDomainEvent;
// This is a domain event: an immutable fact raised by booking reservation
```

Source: `src/Bookify.Application/Abstractions/Messaging/IQuery.cs`

```csharp
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
// Query contract: read side operation, returns Result<T>
```

Source: `src/Bookify.Infrastructure/Outbox/OutboxMessage.cs`

```csharp
public sealed class OutboxMessage {
    public Guid Id { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
}
// Persisted envelope for eventual event publication
```

## Flow of Execution
1. Revisit glossary while reading each chapter.
2. Open linked source paths and find concrete examples.
3. Reinforce understanding by implementing one incremental improvement.

## Common Pitfalls In This Project
- Memorizing terms without tracing real code usage.
- Mixing authentication and authorization language.
- Ignoring naming consistency for commands/queries/events.

## Small Exercises / Checkpoints
1. Write your own one-line definition for each glossary term.
2. Pick three terms and map each one to a specific source file.
3. Build a 2-week learning backlog from the "Next Steps" list below.
4. Re-run tests after each small change.

## Next Learning Steps
1. Implement role + permission policies (chapter 12 blueprints).
2. Add Serilog + health checks + Redis incrementally (chapter 13 blueprints).
3. Introduce integration tests with real PostgreSQL container.
4. Improve concurrency handling across more aggregates.
5. Harden error handling in registration/auth flows.
