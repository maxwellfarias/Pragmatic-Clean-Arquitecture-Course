# 02 - Clean Architecture Fundamentals

## Learning Goals
- Understand the main architectural and design principles used in Bookify.
- Learn how layer boundaries are enforced in code.
- See why this architecture helps maintainability over time.

## Simple Analogy
Imagine a company building:
- The `Domain` is company policy.
- The `Application` is how employees execute policy.
- The `Infrastructure` is vendors/tools used by employees.
- The `API` is customer support desk.

Policy should not depend on vendors. If a vendor changes, policy remains valid.

## Concept Explanation
Core principles used here:
- **Separation of concerns**: each layer has a focused responsibility.
- **Dependency inversion**: outer layers depend on inner abstractions.
- **Explicit boundaries**: domain/business logic is protected from framework details.
- **Testability**: isolated domain and application rules are easier to test.

Bookify has four main projects:
- `Bookify.Domain`
- `Bookify.Application`
- `Bookify.Infrastructure`
- `Bookify.Api`

And tests that enforce architecture conventions.

## Real Code Walkthrough
Source: `test/Bookify.ArchitectureTests/LayerTests.cs`

```csharp
[Fact]
public void DomainLayer_Should_NotHaveDependencyOn_ApplicationLayer()
{
    var result = Types.InAssembly(DomainAssembly)
        .Should()
        .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
        .GetResult();

    Assert.True(result.IsSuccessful); // Domain must stay independent
}
```

Source: `src/Bookify.Api/Program.cs`

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// API references Application + Infrastructure,
// but Domain/Application are not allowed to depend on API.
```

## Flow of Execution
1. HTTP request reaches API layer.
2. API sends command/query to Application layer.
3. Application uses Domain model + repository abstractions.
4. Infrastructure implements abstractions (DB, auth provider, jobs).
5. Response is returned through API.

## Common Pitfalls In This Project
- Putting business rules in controllers.
- Returning EF entities directly from API endpoints.
- Adding Infrastructure dependencies directly to Domain/Application projects.
- Forgetting architecture tests when introducing new conventions.

## Small Exercises / Checkpoints
1. Open each `.csproj` and list direct project references.
2. Explain why `Domain` should not know `Infrastructure`.
3. Add a personal diagram of request flow from API to Domain and back.
4. Read architecture tests and identify what each one protects.
