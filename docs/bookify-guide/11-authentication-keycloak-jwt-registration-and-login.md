# 11 - Authentication: Keycloak, JWT, Registration, and Login

## Learning Goals
- Understand Bookify authentication flow end-to-end.
- Learn Keycloak integration via typed `HttpClient`.
- Understand how JWT bearer authentication is configured.
- Trace registration and login use cases in code.

## Simple Analogy
Think of Keycloak as the passport office:
- Registration creates a person in the passport system.
- Login asks passport office for a temporary travel pass (JWT).
- API checks the pass on each secured endpoint.

## Concept Explanation
### JWT bearer setup
Infrastructure configures ASP.NET authentication middleware and binds options from configuration.

### Registration flow
1. API receives register request.
2. Application builds domain `User`.
3. Infrastructure `AuthenticationService` creates user in Keycloak admin API.
4. App stores returned identity ID in local DB.

### Login flow
1. API receives login request.
2. Application calls `IJwtService`.
3. Infrastructure `JwtService` requests token from Keycloak token endpoint.
4. API returns access token to client.

## Real Code Walkthrough
Source: `src/Bookify.Infrastructure/Authentication/JwtBearerOptionsSetup.cs`

```csharp
public void Configure(JwtBearerOptions options)
{
    options.Audience = _authenticationOptions.Audience;
    options.MetadataAddress = _authenticationOptions.MetadataUrl;
    options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
    options.TokenValidationParameters.ValidIssuer = _authenticationOptions.Issuer;
}
```

Source: `src/Bookify.Infrastructure/DependencyInjection.cs`

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
services.ConfigureOptions<JwtBearerOptionsSetup>();

services.AddHttpClient<IAuthenticationService, AuthenticationService>((sp, client) => {
    var opt = sp.GetRequiredService<IOptions<KeycloakOptions>>().Value;
    client.BaseAddress = new Uri(opt.AdminUrl);
}).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();
```

Source: `src/Bookify.Application/Users/RegisterUser/RegisterUserCommandHandler.cs`

```csharp
var user = User.Create(
    new FirstName(request.FirstName),
    new LastName(request.LastName),
    new Email(request.Email));

var identityId = await _authenticationService.RegisterAsync(
    user,
    request.Password,
    cancellationToken);

user.SetIdentityId(identityId);
_userRepository.Add(user);
await _unitOfWork.SaveChangesAsync();
```

Source: `src/Bookify.Application/Users/LogInUser/LogInUserCommandHandler.cs`

```csharp
var result = await _jwtService.GetAccessTokenAsync(
    request.Email,
    request.Password,
    cancellationToken);

if (result.IsFailure) {
    return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
}

return new AccessTokenResponse(result.Value);
```

Source: `src/Bookify.Api/Http/Keycloack.http`

```http
POST http://localhost:18080/realms/bookify/protocol/openid-connect/token
Content-Type: application/x-www-form-urlencoded

client_id=bookify-auth-client&grant_type=password&client_secret=...&username=test%40test.com&password=Test1234
```

## Flow of Execution
1. Client calls `POST /api/users/register` or `POST /api/users/login`.
2. Users controller dispatches command via MediatR.
3. Application handler coordinates domain and external auth services.
4. Infrastructure calls Keycloak endpoints.
5. API returns user ID (register) or JWT access token (login).

## Common Pitfalls In This Project
- Committing secrets in source control (demo values exist in development settings).
- Missing robust error handling around Keycloak response failures.
- Confusing local domain `UserId` with external identity ID.
- Assuming `UseAuthentication()` alone secures endpoints (authorization attributes still required).

## Small Exercises / Checkpoints
1. Read `appsettings.Development.json` and identify Keycloak URLs.
2. Call login endpoint and decode JWT payload.
3. Trace `IdentityId` from registration response to local DB entity.
4. List what should move to secure secret storage before production.
