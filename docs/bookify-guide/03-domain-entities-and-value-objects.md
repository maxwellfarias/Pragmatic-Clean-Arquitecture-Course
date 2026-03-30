# 03 - Domain Entities and Value Objects

## Learning Goals
- Understand what an Entity is in Domain-Driven Design (DDD).
- Learn why Value Objects reduce primitive obsession.
- See how encapsulation and static factories appear in Bookify.

## Simple Analogy
Think of a passport office:
- A **person** is an Entity (identity matters over time).
- A **name** or **email** is a Value Object (defined by value, not identity).

Even if someone changes city, they remain the same person. That is Entity behavior.

## Concept Explanation
### Entities in Bookify
Entities have identity wrappers (`UserId`, `BookingId`, etc.) and domain behavior.

### Value Objects in Bookify
Small immutable records (for example `Email`, `Money`, `DateRange`) carry meaning and avoid passing raw primitives everywhere.

### Encapsulation and private setters
Most domain properties use private setters to protect invariants and avoid uncontrolled mutation.

### Static factory methods
`User.Create(...)` and `Booking.Reserve(...)` centralize construction rules and side effects (like domain events).

## Real Code Walkthrough
Source: `src/Bookify.Domain/Users/User.cs`

```csharp
public sealed class User : Entity<UserId>
{
    private User() { } // Needed by EF Core materialization

    private User(UserId id, FirstName firstName, LastName lastName, Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public FirstName FirstName { get; private set; } = default!; // Encapsulation
    public LastName LastName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;

    public static User Create(FirstName firstname, LastName lastName, Email email)
    {
        var user = new User(UserId.New(), firstname, lastName, email);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id)); // Factory triggers domain event
        return user;
    }
}
```

Source: `src/Bookify.Domain/Shared/ValueObjects/Money.cs`

```csharp
public record Money(decimal Amount, Currency Currency)
{
    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equals");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }
}
```

Source: `src/Bookify.Domain/Reviews/Rating.cs`

```csharp
public sealed record Rating
{
    public static Result<Rating> Create(int value)
    {
        if (value < 1 || value > 5)
        {
            return Result.Failure<Rating>(Invalid); // Validation in domain value object
        }

        return new Rating(value);
    }
}
```

## Flow of Execution
1. Application layer receives a command.
2. Command handler builds Value Objects / IDs.
3. Domain factory method creates an Entity.
4. Entity raises domain events and enforces rules.
5. Repository persists the aggregate.

## Common Pitfalls In This Project
- Treating Value Objects as just DTO wrappers.
- Bypassing static factories and constructing entities incorrectly.
- Making setters public and weakening invariants.
- Ignoring default `private` constructor purpose for EF Core.

## Small Exercises / Checkpoints
1. Compare `UserId`, `BookingId`, and `ApartmentId` implementations.
2. Add a note: why `Money + Money` validates currencies.
3. Explain difference between `User` entity and `Email` value object in one sentence.
4. Find where `DateRange.Create` enforces business-safe construction.
