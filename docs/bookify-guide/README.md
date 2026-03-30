# Bookify Progressive Technical Guide

## Learning Goals
- Understand the entire Bookify solution step by step, from architecture to API security and testing.
- Learn core .NET and C# concepts in context, using real code from this repository.
- Build confidence to read, modify, and extend a Clean Architecture project.

## Simple Analogy
Think about this guide like a city map for a new place:
- `Domain` is the law of the city (business rules).
- `Application` is city operations (use cases).
- `Infrastructure` is roads, utilities, and service providers (databases, external services).
- `API` is the city front desk (HTTP endpoints).

We start with the map, then we visit each district.

## Concept Explanation
This guide is intentionally **progressive** and **beginner-friendly**:
- Each file follows the same chapter structure.
- Each chapter has real code snippets with comments.
- Missing course topics are still covered with:
  - current Bookify status, and
  - extension blueprints (`Proposed Extension (Not Implemented Yet)`).

## Real Code Walkthrough
Source: `src/Bookify.Api/Program.cs`

```csharp
using Bookify.Application;
using Bookify.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();      // Registers use-case handlers, validators, pipeline behaviors
builder.Services.AddInfrastructure(builder.Configuration); // Registers DB, auth, repositories, jobs

var app = builder.Build();

app.UseAuthentication(); // Validates JWT bearer tokens
app.UseAuthorization();  // Applies authorization rules

app.MapControllers();    // MVC controllers
app.MapBookingEndpoints(); // Minimal API endpoints

app.Run();
```

## Flow of Execution
1. Open [01-introduction-and-prerequisites.md](./01-introduction-and-prerequisites.md).
2. Move to architecture in [02-clean-architecture-fundamentals.md](./02-clean-architecture-fundamentals.md).
3. Deep-dive into Domain files [03](./03-domain-entities-and-value-objects.md), [04](./04-domain-events-repositories-and-unit-of-work.md), [05](./05-domain-services-double-dispatch-result-and-errors.md).
4. Continue with Application [06](./06-application-layer-cqrs-mediatr-abstractions.md) and [07](./07-application-commands-queries-pipeline-validation-logging.md).
5. Study Infrastructure [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) and [09](./09-infrastructure-outbox-quartz-and-optimistic-concurrency.md).
6. Study Presentation and Security [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md), [11](./11-authentication-keycloak-jwt-registration-and-login.md), [12](./12-authorization-roles-permissions-and-resource-based-authorization.md), [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md).
7. Finish with testing and glossary [14](./14-testing-domain-application-architecture-and-missing-integration-functional.md), [15](./15-glossary-and-next-learning-steps.md).
8. Use the full matrix in [00-course-coverage-matrix.md](./00-course-coverage-matrix.md).

## Common Pitfalls In This Project
- Assuming every course topic is fully implemented in this snapshot (some are partial/missing).
- Confusing `Domain` entities with API request/response models.
- Expecting query handlers to use EF Core (Bookify uses Dapper for read models).
- Ignoring warnings: there are nullable warnings that are educational for junior devs.

## Small Exercises / Checkpoints
1. Open `Program.cs` and identify where each layer is wired.
2. Find one controller endpoint and one minimal endpoint in the API project.
3. Compare where write operations and read operations are implemented.
4. Read the course matrix file and pick one topic to study deeply first.
