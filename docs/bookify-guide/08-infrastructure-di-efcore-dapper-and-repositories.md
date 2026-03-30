# 08 - Infrastructure: DI, EF Core, Dapper, and Repositories

## Learning Goals
- Understand how Infrastructure wires technical services.
- Learn how EF Core and Dapper coexist in this architecture.
- Read entity configuration patterns for value objects and IDs.
- Understand repository implementation details.

## Simple Analogy
Think of Infrastructure as the logistics team:
- Domain/Application define what must happen.
- Infrastructure decides which truck, road, and warehouse are used.

Logistics can change without changing company policy.

## Concept Explanation
### Dependency Injection in Infrastructure
`AddInfrastructure(...)` composes technical dependencies:
- DB context,
- repositories,
- SQL connection factory,
- authentication clients,
- background job scheduler.

### EF Core for writes
Aggregate persistence and transactional operations are handled through `ApplicationDbContext` + repositories.

### Dapper for reads
Query handlers use raw SQL projection via `ISqlConnectionFactory` for performant read models.

### Entity configurations
Bookify maps strongly typed IDs and value objects using `HasConversion` and `OwnsOne`.

## Real Code Walkthrough
Source: `src/Bookify.Infrastructure/DependencyInjection.cs`

```csharp
private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
{
    var connectionsSTring =
        configuration.GetConnectionString("Database") ??
        throw new ArgumentNullException(nameof(configuration));

    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionsSTring).UseSnakeCaseNamingConvention();
    });

    services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionsSTring));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IApartmentRepository, ApartmentRepository>();
    services.AddScoped<IBookingRepository, BookingRepository>();

    services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
}
```

Source: `src/Bookify.Infrastructure/Configurations/ApartmentConfiguration.cs`

```csharp
builder.Property(x => x.Id)
    .ValueGeneratedNever()
    .HasConversion(
        (x) => x.Value,
        (g) => ApartmentId.FromValue(g)
    );

builder.OwnsOne(c => c.Price, bx =>
{
    bx.Property(m => m.Currency)
      .HasConversion(cc => cc.Code, code => Currency.FromCode(code));
});
```

Source: `src/Bookify.Infrastructure/Repositories/Repository.cs`

```csharp
internal abstract class Repository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : class
{
    protected readonly ApplicationDbContext _db;

    public async Task<TEntity?> GetByIdAsync(TEntityId Id, CancellationToken ct = default)
    {
        return await _db.Set<TEntity>()
            .FirstOrDefaultAsync(x => x.Id == Id, ct); // Shared base query logic
    }
}
```

## Flow of Execution
1. `Program.cs` calls `AddInfrastructure(configuration)`.
2. Infrastructure registers DB and repository services.
3. Application handlers request interfaces (DI resolves concrete implementations).
4. Write paths use EF Core repositories + `IUnitOfWork`.
5. Read paths use Dapper through `ISqlConnectionFactory`.

## Common Pitfalls In This Project
- Using EF for all reads when projection SQL is simpler/faster.
- Returning tracked entities to API contracts.
- Forgetting to configure value-object conversions.
- Typos in connection string key names or environment-specific config.

## Small Exercises / Checkpoints
1. Trace one dependency registration from interface to concrete class.
2. Find all `OwnsOne(...)` mappings and explain why they are needed.
3. Compare one EF write path and one Dapper read path.
4. Explain why `IUnitOfWork` is resolved as `ApplicationDbContext`.
