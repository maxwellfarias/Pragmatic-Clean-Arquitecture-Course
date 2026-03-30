# 14 - Testing: Domain, Application, Architecture, and Missing Integration/Functional Tests

## Learning Goals
- Understand existing test layers in Bookify.
- Learn what each test type validates.
- Identify gaps (integration and functional tests) and how to add them.

## Simple Analogy
Testing is like quality control in a factory:
- Unit tests inspect one part in isolation.
- Architecture tests verify factory rules.
- Integration tests verify machine interactions.
- Functional tests verify full customer journey.

## Concept Explanation
### Domain unit tests (implemented)
Domain tests validate entity behavior and domain events without infrastructure.

### Application unit tests (implemented)
Application tests mock dependencies and verify use-case outcomes.

### Architecture tests (implemented)
Architecture tests assert layer dependency rules and naming conventions.

### Integration/functional tests (not implemented yet)
No dedicated integration or functional test projects currently exist in this repo snapshot.

## Real Code Walkthrough
Source: `test/Bookify.Domain.UnitTests/Users/UserBaseTests.cs`

```csharp
[Fact]
public void Create_Should_Raise_UseCreatedDomainEvent()
{
    var firstName = new FirstName("first");
    var lastName = new LastName("last");
    var email = new Email("test@test.com");

    var user = User.Create(firstName, lastName, email);

    var userCreatedDomainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);
    Assert.Equal(user.Id, userCreatedDomainEvent.UserId); // Domain behavior assertion
}
```

Source: `test/Bookify.Application.UnitTests/Bookings/ReserveBookingTests.cs`

```csharp
userRepositoryMock
    .Setup(u => u.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync((User?)null);

var result = await handler.Handle(command, default);

Assert.Equal(UserErrors.NotFound, result.Error); // Application behavior assertion
```

Source: `test/Bookify.ArchitectureTests/LayerTests.cs`

```csharp
var result = Types.InAssembly(ApplicationAssembly)
    .Should()
    .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
    .GetResult();

Assert.True(result.IsSuccessful); // Architectural boundary assertion
```

### Proposed Extension (Not Implemented Yet)

```csharp
// Proposed Extension (Not Implemented Yet)
// Integration test skeleton using WebApplicationFactory + test DB
public class ReserveBookingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ReserveBooking_ShouldPersistBooking_WhenRequestIsValid()
    {
        // Arrange test host + database
        // Act call HTTP endpoint
        // Assert persisted row + expected response
    }
}
```

```csharp
// Proposed Extension (Not Implemented Yet)
// Functional test skeleton for login + authorized endpoint flow
[Fact]
public async Task SearchApartments_ShouldReturn200_WhenBearerTokenIsValid()
{
    // 1) Register or login user
    // 2) Acquire JWT
    // 3) Call secured endpoint with token
    // 4) Assert successful response
}
```

## Flow of Execution
1. Run `dotnet test PragmaticCA.sln`.
2. Test runner executes Domain unit tests.
3. Test runner executes Application unit tests.
4. Test runner executes Architecture tests.
5. Future: add integration/functional projects into solution pipeline.

## Common Pitfalls In This Project
- Overusing mocks in scenarios that need integration tests.
- Treating architecture tests as optional.
- Not asserting negative cases (validation, unauthorized access, overlap conflicts).
- Missing test data isolation strategy for future integration tests.

## Small Exercises / Checkpoints
1. Add one more domain unit test for `Rating.Create` invalid value.
2. Add one application unit test for overlap failure path.
3. Propose integration test project structure and naming.
4. Record current baseline: on March 29, 2026, `dotnet test PragmaticCA.sln` passed with 10 tests.
