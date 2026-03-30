# 06 - Application Layer, CQRS, and MediatR Abstractions

## Learning Goals
- Understand the purpose of the Application layer in Clean Architecture.
- Learn how Bookify uses CQRS contracts (`ICommand`, `IQuery`, handlers).
- Understand MediatR registration and pipeline behavior wiring.

## Simple Analogy
Imagine a call center:
- A **command** is a request that changes data ("reserve a booking").
- A **query** is a request that reads data ("get booking details").

Different request types can have different optimization strategies.

## Concept Explanation
The Application layer is the orchestration layer:
- It coordinates domain model operations.
- It does not own persistence details directly.
- It defines contracts for commands/queries and delegates handling.

Bookify’s CQRS abstractions:
- `ICommand` and `ICommand<T>` return `Result`.
- `IQuery<T>` returns `Result<T>`.
- Handlers map to MediatR `IRequestHandler`.

## Real Code Walkthrough
Source: `src/Bookify.Application/Abstractions/Messaging/ICommand.cs`

```csharp
public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}

public interface IBaseCommand
{
    // Marker interface used by pipeline behaviors (logging/validation)
}
```

Source: `src/Bookify.Application/Abstractions/Messaging/IQuery.cs`

```csharp
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
```

Source: `src/Bookify.Application/DependencyInjection.cs`

```csharp
services.AddMediatR(m =>
{
    m.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

    // Pipeline order matters: logging then validation in this project setup
    m.AddOpenBehavior(typeof(LoggingBehavior<,>));
    m.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
```

## Flow of Execution
1. API endpoint creates command/query object.
2. Endpoint calls `_sender.Send(request)` (MediatR).
3. Pipeline behaviors execute.
4. Matching handler runs use-case logic.
5. Handler returns `Result`/`Result<T>`.

## Common Pitfalls In This Project
- Mixing command and query concerns in one handler.
- Returning domain entities directly for read endpoints.
- Forgetting to implement `IBaseCommand` through command abstractions.
- Ignoring behavior execution order.

## Small Exercises / Checkpoints
1. Find all handlers in `src/Bookify.Application`.
2. Label each one as command or query.
3. Explain why queries use Dapper in this codebase.
4. Write a short note on why marker interfaces are useful for pipelines.
