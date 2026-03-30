# 12 - Authorization: Roles, Permissions, and Resource-Based Authorization

## Learning Goals
- Understand current authorization state in Bookify.
- Learn role-based, permission-based, and resource-based authorization patterns.
- Identify safe extension points in current code.

## Simple Analogy
Authentication is checking your badge at building entrance.
Authorization is checking which rooms your badge can open.

- Role-based: "Manager badge opens finance room."
- Permission-based: "Badge has `bookings.write` claim."
- Resource-based: "You can open this room only if it is assigned to you."

## Concept Explanation
### Current state in Bookify
The project currently uses broad endpoint protection:
- `[Authorize]` on controller.
- `.RequireAuthorization()` on minimal API group.
- `[AllowAnonymous]` on register/login endpoints.

Advanced policy layers (roles/permissions/resource handlers) are not fully implemented yet.

### Role-based authorization
Use role claims and endpoint policies to gate features by role.

### Permission-based authorization
Model fine-grained actions as permissions (`bookings.read`, `bookings.reserve`).

### Resource-based authorization
Authorizes access to a specific entity instance (example: user can access only own booking).

## Real Code Walkthrough
Source: `src/Bookify.Api/Controllers/Apartments/ApartmentsController.cs`

```csharp
[Authorize]
[ApiController]
[Route("api/apartments")]
public class ApartmentsController : ControllerBase
{
    // All endpoints here require authenticated user
}
```

Source: `src/Bookify.Api/Controllers/Bookings/BookingsEndpoints.cs`

```csharp
var builder = Group
    .MapGroup("api/bookings")
    .RequireAuthorization(); // Protects the whole group
```

Source: `src/Bookify.Api/Controllers/Users/UsersController.cs`

```csharp
[AllowAnonymous]
[HttpPost("register")]
public async Task<IActionResult> Register(...) { ... }

[AllowAnonymous]
[HttpPost("login")]
public async Task<IActionResult> LogIn(...) { ... }
```

### Proposed Extension (Not Implemented Yet)

```csharp
// Proposed Extension (Not Implemented Yet)
// Role policy registration in Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("CanReserveBooking", policy =>
        policy.RequireClaim("permission", "bookings.reserve"));
});
```

```csharp
// Proposed Extension (Not Implemented Yet)
// Resource-based requirement for booking ownership
public sealed class BookingOwnerRequirement : IAuthorizationRequirement { }
```

```csharp
// Proposed Extension (Not Implemented Yet)
// Resource handler skeleton
public sealed class BookingOwnerHandler : AuthorizationHandler<BookingOwnerRequirement, Booking>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BookingOwnerRequirement requirement,
        Booking resource)
    {
        var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == resource.UserId.Value.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

## Flow of Execution
1. `UseAuthentication()` validates JWT.
2. `UseAuthorization()` checks endpoint authorization metadata.
3. If policy/attribute requirements pass, endpoint executes.
4. Otherwise framework returns 401 or 403.

## Common Pitfalls In This Project
- Assuming authentication automatically means proper authorization.
- Putting business ownership checks only in controllers.
- Designing permission names without a consistent taxonomy.
- Forgetting to test 401 vs 403 scenarios.

## Small Exercises / Checkpoints
1. Add a permission matrix for current endpoints.
2. Draft one role policy and one permission policy for Bookify.
3. Define ownership rule for "get booking by id".
4. List integration tests you would create for authorization.
