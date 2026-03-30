# 05 - Domain Services, Double Dispatch, Result, and Errors

## Learning Goals
- Understand the Domain Service role in Bookify (`PricingService`).
- Learn how `Result` models success/failure without exceptions for business outcomes.
- Understand centralized domain error catalogs.
- Learn where double-dispatch ideas appear and how to extend them.

## Simple Analogy
Think of booking price calculation like hiring an external appraiser:
- The apartment and date range describe the case.
- The appraiser (domain service) computes value using business rules.

The appraiser is independent logic that doesn’t naturally belong inside one entity only.

## Concept Explanation
### Domain Service
`PricingService` computes booking cost from apartment pricing, dates, and amenities.

### Result Pattern
`Result` and `Result<T>` represent business success/failure explicitly.
This avoids throwing exceptions for expected business cases (like overlap).

### Domain Error Dictionary
Each module has static error definitions (`BookingErrors`, `UserErrors`, etc.).
This keeps error codes/messages consistent.

### Double Dispatch in this codebase
Bookify uses a rich model where interactions happen across domain types (`Booking.Reserve(apartment, ...)`, `Review.Create(booking, ...)`).
It is not a classic visitor-based double dispatch, but the intent is similar: behavior depends on multiple domain objects.

## Real Code Walkthrough
Source: `src/Bookify.Domain/Bookings/PricingService.cs`

```csharp
public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
{
    var currency = apartment.Price.Currency;

    var priceForPeriod = new Money(
        apartment.Price.Amount * period.LengthInDays,
        currency);

    // Domain service applies amenity-dependent price rules
    decimal percentageUpCharge = 0;
    foreach (var amenity in apartment.Amenities)
    {
        percentageUpCharge += amenity switch
        {
            Amenity.GardenView or Amenity.MountainView => 0.5m,
            Amenity.AirConditioning => 0.01m,
            Amenity.Parking => 0.01m,
            _ => 0
        };
    }

    return new PricingDetails(priceForPeriod, apartment.CleaningFee, Money.Zero(), priceForPeriod);
}
```

Source: `src/Bookify.Domain/Abstractions/Result.cs`

```csharp
public static Result<T> Failure<T>(Error error) => new(default!, false, error);

public static Result<T> Create<T>(T? value) =>
    value is not null ? Success(value) : Failure<T>(Error.NullValue);
```

Source: `src/Bookify.Domain/Bookings/BookingErrors.cs`

```csharp
public static readonly Error Overlap = new(
    "Booking.Overlap",
    "The booking is overlaping with an existing one");
```

Source: `src/Bookify.Domain/Reviews/Review.cs`

```csharp
public static Result<Review> Create(
    Booking booking,
    Rating rating,
    Comment comment,
    DateTime createdOnUtc)
{
    if (booking.Status != BookingStatus.Completed)
    {
        return Result.Failure<Review>(ReviewErrors.NotEligible); // Business failure via Result
    }

    return new Review(
        ReviewId.New(),
        booking.ApartmentId,
        booking.Id,
        booking.UserId,
        rating,
        comment,
        createdOnUtc);
}
```

### Proposed Extension (Not Implemented Yet)

```csharp
// Proposed Extension (Not Implemented Yet)
// A more explicit double-dispatch style where apartment can calculate
// amenity up-charge strategy by booking context.
public interface IAmenityPricingRule
{
    Money Calculate(Apartment apartment, DateRange period);
}
```

## Flow of Execution
1. Application handler loads entities/value objects.
2. Handler calls `Booking.Reserve(..., pricingService)`.
3. Domain service returns `PricingDetails`.
4. Domain methods return `Result` when business rule can fail.
5. Application maps `Result.Error` to HTTP response.

## Common Pitfalls In This Project
- Using exceptions for normal business failures instead of `Result`.
- Creating ad-hoc error strings in handlers instead of error catalogs.
- Putting pricing logic in handler/controller instead of domain service.
- Assuming current implementation is full classic double dispatch.

## Small Exercises / Checkpoints
1. Add a table of all current error codes from Domain modules.
2. Trace where `BookingErrors.Overlap` is returned to API.
3. Compare exception-based failure vs `Result`-based failure.
4. Propose one improvement to `PricingService` while keeping domain purity.
