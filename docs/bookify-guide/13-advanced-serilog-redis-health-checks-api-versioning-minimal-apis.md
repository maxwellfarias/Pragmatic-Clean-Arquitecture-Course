# 13 - Advanced Topics: Serilog, Redis, Health Checks, API Versioning, Outbox, Minimal APIs

## Learning Goals
- Understand which advanced topics are already present in Bookify.
- Learn implementation blueprints for missing advanced topics.
- Connect advanced platform concerns with Clean Architecture boundaries.

## Simple Analogy
Think of this chapter as adding professional operations tooling to a race car:
- Outbox is reliability.
- Structured logging is telemetry.
- Caching is turbo boost for repeated reads.
- Health checks are dashboard warning lights.
- API versioning is backward-compatible gearbox.

## Concept Explanation
### Already implemented
- Transactional Outbox + Quartz background processing.
- Minimal APIs for booking endpoints.

### Not implemented yet (blueprints provided)
- Serilog + Seq structured logging.
- Redis distributed caching.
- Health checks endpoints.
- API versioning strategy.

## Real Code Walkthrough
Source: `src/Bookify.Api/Controllers/Bookings/BookingsEndpoints.cs`

```csharp
public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder Group) {

        var builder = Group
            .MapGroup("api/bookings")
            .RequireAuthorization();

        builder.MapGet("{id}", GetBooking);
        builder.MapPost("", ReserveBooking);

        return Group;
    }
}
```

Source: `src/Bookify.Infrastructure/Outbox/ProcessOutboxMessagesJob.cs`

```csharp
_logger.LogInformation("Beginning to process outbox messages");

using var connection = _sqlConnectionFactory.CreateConnection();
using var transaction = connection.BeginTransaction();

var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

// Each outbox message is published and marked as processed
```

### Proposed Extension (Not Implemented Yet)

```csharp
// Proposed Extension (Not Implemented Yet)
// Serilog + Seq registration in Program.cs
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));
```

```csharp
// Proposed Extension (Not Implemented Yet)
// Redis distributed cache registration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "bookify:";
});
```

```csharp
// Proposed Extension (Not Implemented Yet)
// Health checks for API and DB
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

app.MapHealthChecks("/health");
```

```csharp
// Proposed Extension (Not Implemented Yet)
// API versioning skeleton
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

## Flow of Execution
1. Minimal API endpoints serve lightweight HTTP routes.
2. Outbox job improves eventual consistency and resilience.
3. Future advanced additions improve observability, performance, and operability.
4. Clean Architecture boundaries remain unchanged while platform capabilities grow.

## Common Pitfalls In This Project
- Adding advanced tooling directly inside Domain layer.
- Enabling cache without cache invalidation strategy.
- Exposing health check details publicly without security considerations.
- Shipping breaking API changes without versioning.

## Small Exercises / Checkpoints
1. Draft a `docker-compose` extension adding Redis and Seq containers.
2. Pick one query endpoint and design a cache key strategy.
3. Define which health checks should be liveness vs readiness.
4. Propose a versioning policy (`v1`, `v2`) for future breaking changes.
