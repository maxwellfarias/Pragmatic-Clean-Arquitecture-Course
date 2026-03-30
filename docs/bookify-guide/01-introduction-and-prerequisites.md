# 01 - Introduction and Prerequisites

## Learning Goals
- Understand what Bookify solves and why this project is a good Clean Architecture learning case.
- Identify who this learning path is for.
- Set up realistic prerequisites before deep implementation study.

## Simple Analogy
Learning this project is like learning to cook in a professional kitchen:
- First, know the menu (what we are building).
- Then, know your role and tools (prerequisites).
- Finally, learn each station in order (layers/chapters).

## Concept Explanation
### What we are building
Bookify is an apartment booking API with:
- apartment search,
- booking reservation,
- user registration/login,
- Keycloak-based authentication,
- domain-event outbox processing.

### Who this guide is for
- Developers new to C#/.NET who want practical architecture patterns.
- Backend developers migrating from CRUD-only APIs to richer domain modeling.
- Students who prefer reading code + guided explanations instead of only video lessons.

### Prerequisites (recommended)
- Basic OOP ideas (class, record, interface, method).
- Basic HTTP and REST concepts.
- .NET SDK installed (the solution targets `.NET 9`).
- Docker/Docker Compose to run Postgres + Keycloak.
- Basic SQL reading ability.

## Real Code Walkthrough
Source: `src/Bookify.Api/Bookify.Api.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <!-- This project uses .NET 9, so install matching SDK -->
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

Source: `docker-compose.yml`

```yaml
services:
  bookify-db:
    image: postgres:latest
    # Database used by EF Core + Dapper

  bookify-idp:
    image: quay.io/keycloak/keycloak:latest
    command: start-dev --import-realm
    # Identity provider used for JWT authentication
```

## Flow of Execution
1. The API process starts.
2. Infrastructure services connect Bookify to database and identity provider.
3. End users call endpoints for registration, login, search, and bookings.
4. Domain rules validate business behavior.
5. Persistence and background jobs handle reliability concerns.

## Common Pitfalls In This Project
- Starting implementation details before understanding the layer boundaries.
- Skipping Docker setup and then assuming auth/database code is broken.
- Treating this as just "controllers + EF" (it is intentionally richer than that).

## Small Exercises / Checkpoints
1. Confirm your machine runs `dotnet --version` with a .NET 9 SDK.
2. Start reading `Program.cs` to see startup wiring.
3. Open `docker-compose.yml` and identify API, DB, and identity provider services.
4. Write one paragraph: what business problem Bookify solves.
