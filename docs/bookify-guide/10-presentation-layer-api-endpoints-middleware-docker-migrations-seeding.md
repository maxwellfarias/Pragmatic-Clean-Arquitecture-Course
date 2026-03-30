# 10 - Presentation Layer: API, Middleware, Docker, Migrations, and Seeding

## Learning Goals
- Understand startup pipeline and endpoint mapping in `Bookify.Api`.
- Learn differences between Controllers and Minimal APIs in this project.
- Understand global exception middleware behavior.
- Learn local run flow with Docker Compose, migrations, and seed data.

## Simple Analogy
The Presentation layer is the front desk of a hotel:
- It receives requests.
- It validates identity and routes users to internal teams.
- It returns friendly answers in HTTP format.

The front desk should not decide business policy.

## Concept Explanation
### Startup pipeline
`Program.cs` composes API features, Swagger, DI, auth middleware, and endpoint maps.

### Controllers + Minimal APIs
Bookify uses both styles:
- controllers (`UsersController`, `ApartmentsController`),
- minimal endpoints (`BookingsEndpoints`).

### Global exception handling
`ExceptionHandlingMiddleware` converts known exceptions into `ProblemDetails` JSON.

### Docker and initial data
- `docker-compose.yml` provides API, Postgres, Keycloak.
- On development startup, app applies migrations and seeds apartment data.

## Real Code Walkthrough
Source: `src/Bookify.Api/Program.cs`

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });

    app.ApplyMigrations(); // Auto-apply EF migrations in dev
    app.SeedData();        // Seed sample apartments
}

app.UseCustomExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBookingEndpoints();
```

Source: `src/Bookify.Api/Controllers/Bookings/BookingsEndpoints.cs`

```csharp
var builder = Group
    .MapGroup("api/bookings")
    .RequireAuthorization(); // Minimal API group protected by auth

builder.MapGet("{id}", GetBooking);
builder.MapPost("", ReserveBooking);
```

Source: `src/Bookify.Api/Middleware/ExceptionHandlingMiddleware.cs`

```csharp
catch (Exception exception)
{
    _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

    var exceptionDetails = GetExceptionDetails(exception);

    var problemDetails = new ProblemDetails
    {
        Status = exceptionDetails.Status,
        Type = exceptionDetails.Type,
        Title = exceptionDetails.Title,
        Detail = exceptionDetails.Detail,
    };

    await context.Response.WriteAsJsonAsync(problemDetails); // Uniform API error format
}
```

Source: `src/Bookify.Api/Extensions/SeedDataExtensions.cs`

```csharp
for (var i = 0; i < 100; i++)
{
    apartments.Add(new
    {
        Id = Guid.NewGuid(),
        Name = faker.Company.CompanyName(),
        // Faker-generated sample data for local development
    });
}

connection.Execute(sql, apartments); // Bulk insert sample apartments
```

## Flow of Execution
1. API process starts and builds middleware pipeline.
2. In development, migrations and seed scripts run.
3. Client sends HTTP request to controller or minimal endpoint.
4. Endpoint sends command/query to Application via MediatR.
5. Middleware handles unexpected exceptions and returns consistent responses.

## Common Pitfalls In This Project
- Running seeds repeatedly without idempotency checks.
- Exposing internal exception details in production.
- Mixing endpoint shape and business rule decisions in controllers.
- Not securing endpoints consistently across controller/minimal API styles.

## Small Exercises / Checkpoints
1. Compare `/api/bookings_c` controller route with `api/bookings` minimal route.
2. Trigger a validation error and inspect `ProblemDetails` response.
3. Inspect `docker-compose.override.yml` and identify dev-only API settings.
4. Explain when `ApplyMigrations()` should and should not run.
