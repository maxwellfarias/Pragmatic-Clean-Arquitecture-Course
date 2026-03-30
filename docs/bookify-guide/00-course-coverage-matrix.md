# Course Coverage Matrix

## Learning Goals
- Verify that every course topic is covered by this guide.
- Quickly see whether each topic is implemented, partially implemented, or blueprint-only in this repository snapshot.
- Navigate directly to the right chapter for each concept.

## Simple Analogy
Use this file like an airport departures board:
- `Implemented` = flight is boarding now (you can inspect real code).
- `Partial` = flight delayed (some code exists, but incomplete).
- `Blueprint` = route planned, not flying yet (design guidance only).

## Concept Explanation
Coverage status legend:
- `Implemented`: there is real code in this repo.
- `Partial`: code exists but is incomplete / simplified.
- `Blueprint`: not implemented in code yet; guide provides extension design.

### Matrix
| Course Topic | Coverage Location | Repo Status |
|---|---|---|
| Welcome | [01](./01-introduction-and-prerequisites.md) | Implemented (docs context) |
| What you will learn | [01](./01-introduction-and-prerequisites.md) | Implemented (docs context) |
| Who this course is for | [01](./01-introduction-and-prerequisites.md) | Implemented (docs context) |
| Prerequisites | [01](./01-introduction-and-prerequisites.md) | Implemented (docs context) |
| Architectural principles & design principles | [02](./02-clean-architecture-fundamentals.md) | Implemented |
| Clean Architecture layers | [02](./02-clean-architecture-fundamentals.md) | Implemented |
| What we are building & why | [01](./01-introduction-and-prerequisites.md), [02](./02-clean-architecture-fundamentals.md) | Implemented |
| Domain chapter intro | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| Creating Domain project | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| First domain entity | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| Entity in DDD | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| Value Objects & primitive obsession | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| Private setters & encapsulation | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| Static Factory pattern | [03](./03-domain-entities-and-value-objects.md) | Implemented |
| Domain Events pattern | [04](./04-domain-events-repositories-and-unit-of-work.md) | Implemented |
| Repositories and Unit of Work | [04](./04-domain-events-repositories-and-unit-of-work.md) | Implemented |
| Booking entity | [03](./03-domain-entities-and-value-objects.md), [05](./05-domain-services-double-dispatch-result-and-errors.md) | Implemented |
| Domain service | [05](./05-domain-services-double-dispatch-result-and-errors.md) | Implemented |
| Double Dispatch | [05](./05-domain-services-double-dispatch-result-and-errors.md) | Partial (conceptual pattern present indirectly) |
| Result class | [05](./05-domain-services-double-dispatch-result-and-errors.md) | Implemented |
| Domain errors dictionary | [05](./05-domain-services-double-dispatch-result-and-errors.md) | Implemented |
| Application chapter intro | [06](./06-application-layer-cqrs-mediatr-abstractions.md) | Implemented |
| Creating Application project | [06](./06-application-layer-cqrs-mediatr-abstractions.md) | Implemented |
| DI and MediatR | [06](./06-application-layer-cqrs-mediatr-abstractions.md) | Implemented |
| CQRS abstractions | [06](./06-application-layer-cqrs-mediatr-abstractions.md) | Implemented |
| Commands + domain + repos | [07](./07-application-commands-queries-pipeline-validation-logging.md) | Implemented |
| Domain event handler | [07](./07-application-commands-queries-pipeline-validation-logging.md) | Implemented |
| Queries with Dapper | [07](./07-application-commands-queries-pipeline-validation-logging.md) | Implemented |
| Logging cross-cutting concern | [07](./07-application-commands-queries-pipeline-validation-logging.md) | Implemented |
| Validation pipeline | [07](./07-application-commands-queries-pipeline-validation-logging.md) | Implemented |
| Infrastructure chapter intro | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Creating Infrastructure project | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Infrastructure DI | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Add EF Core | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Configure EF Core with DI | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Domain entity configurations | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Apply configurations automatically | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Repositories via generic base | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| SQL connection factory | [08](./08-infrastructure-di-efcore-dapper-and-repositories.md) | Implemented |
| Publish domain events inside UoW | [09](./09-infrastructure-outbox-quartz-and-optimistic-concurrency.md) | Implemented (via Outbox) |
| Optimistic concurrency | [09](./09-infrastructure-outbox-quartz-and-optimistic-concurrency.md) | Partial (row version only on Apartment) |
| Presentation chapter intro | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Creating Presentation project | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Service configuration via DI | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Controllers and endpoints | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Docker Compose & run API | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| EF migrations and apply | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Seeding initial data | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Global exception middleware | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md) | Implemented |
| Authentication chapter intro | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Keycloak setup | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Keycloak admin panel overview | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Partial (docs + HTTP flows, no UI assets) |
| Obtain JWTs with Keycloak | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Configure auth middleware/services | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Configure JWT options | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Authentication in action | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| User registration feature | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Partial (error handling/polish gaps) |
| Register user endpoint | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Authentication service | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| Typed HTTP client for auth service | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| User registration with Keycloak demo | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented (local run) |
| User login feature | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented |
| User login with Keycloak demo | [11](./11-authentication-keycloak-jwt-registration-and-login.md) | Implemented (local run) |
| Role-based authorization | [12](./12-authorization-roles-permissions-and-resource-based-authorization.md) | Blueprint |
| Permission-based authorization | [12](./12-authorization-roles-permissions-and-resource-based-authorization.md) | Blueprint |
| Resource-based authorization | [12](./12-authorization-roles-permissions-and-resource-based-authorization.md) | Blueprint |
| Structured logging with Serilog/Seq | [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md) | Blueprint |
| Distributed caching with Redis | [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md) | Blueprint |
| Health checks | [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md) | Blueprint |
| API versioning | [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md) | Blueprint |
| Transactional Outbox | [09](./09-infrastructure-outbox-quartz-and-optimistic-concurrency.md), [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md) | Implemented |
| Minimal APIs | [10](./10-presentation-layer-api-endpoints-middleware-docker-migrations-seeding.md), [13](./13-advanced-serilog-redis-health-checks-api-versioning-minimal-apis.md) | Implemented |
| Domain unit testing | [14](./14-testing-domain-application-architecture-and-missing-integration-functional.md) | Implemented |
| Application unit testing | [14](./14-testing-domain-application-architecture-and-missing-integration-functional.md) | Implemented |
| Integration testing | [14](./14-testing-domain-application-architecture-and-missing-integration-functional.md) | Blueprint |
| Functional testing | [14](./14-testing-domain-application-architecture-and-missing-integration-functional.md) | Blueprint |
| Architecture testing | [14](./14-testing-domain-application-architecture-and-missing-integration-functional.md) | Implemented |

## Real Code Walkthrough
Source: `src/Bookify.Infrastructure/ApplicationDbContext.cs`

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    try
    {
        AddDomainEventsAsOutboxMessages(); // Converts domain events into durable outbox rows

        var result = await base.SaveChangesAsync(ct); // Persists entities + outbox in one transaction
        return result;
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // Maps EF concurrency exception into app-specific exception type
        throw new ConcurrencyException("Concurrency Exception occurred.", ex);
    }
}
```

## Flow of Execution
1. Pick any course bullet from the matrix.
2. Open the linked chapter.
3. Inspect the corresponding source file mentioned in the snippet source path.
4. If status is `Blueprint`, use the proposed extension section in that chapter.

## Common Pitfalls In This Project
- Treating `Partial` as production-ready.
- Ignoring `Blueprint` sections because no code exists yet.
- Assuming matrix status never changes (it should evolve as you implement features).

## Small Exercises / Checkpoints
1. Find one `Implemented` topic and confirm the snippet in source code.
2. Find one `Partial` topic and write what is missing.
3. Find one `Blueprint` topic and identify extension points in current code.
