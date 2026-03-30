# 07 - Commands, Queries, Pipeline, Validation, and Logging

## Learning Goals
- Follow a full command use case from API to Domain and back.
- Understand query handlers with Dapper for read performance.
- Learn cross-cutting behaviors: validation and logging.
- See domain event handlers in Application layer.

## Simple Analogy
Think of an airport process:
- Validation is security check before boarding.
- Logging is flight tracking.
- Command handler is the crew executing the mission.
- Query handler is the information desk.

## Concept Explanation
### Commands (write side)
`ReserveBookingCommandHandler` performs orchestration:
- load user/apartment,
- validate overlap,
- reserve booking,
- commit unit of work.

### Queries (read side)
`GetBookingQueryHandler` and `SearchApartmentsQueryHandler` use Dapper SQL to project response models quickly.

### Validation behavior
`ValidationBehavior` runs FluentValidation validators before command handlers.

### Logging behavior
`LoggingBehavior` logs command start/success/failure consistently.

### Domain event handlers
Application handles domain notifications (for example, sending booking confirmation emails).

## Real Code Walkthrough
Source: `src/Bookify.Application/Bookings/ReserveBooking/ReserveBookingCommandHandler.cs`

```csharp
public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken ct)
{
    var user = await _userRepository.GetByIdAsync(UserId.FromValue(request.UserId), ct);
    if (user is null)
        return Result.Failure<Guid>(UserErrors.NotFound); // Business-safe failure

    var apartment = await _apartmentRepository.GetByIdAsync(ApartmentId.FromValue(request.ApartmentId), ct);
    if (apartment is null)
        return Result.Failure<Guid>(ApartmentErrors.NotFound);

    var duration = DateRange.Create(request.StarDate, request.EndDate);

    if (await _bookingRepository.IsOverlappingAsync(apartment, duration, ct))
        return Result.Failure<Guid>(BookingErrors.Overlap);

    var booking = Booking.Reserve(apartment, user.Id, duration, _dateTimeProvider.UtcNow, _pricingService);
    _bookingRepository.Add(booking);
    await _unitOfWork.SaveChangesAsync(ct);

    return booking.Id.Value;
}
```

Source: `src/Bookify.Application/Bookings/GetBooking/GetBookingQueryHandler.cs`

```csharp
const string sql = """
    SELECT
        id AS Id,
        apartment_id AS ApartmentId,
        user_id AS UserId,
        status AS Status
    FROM bookings
    WHERE id = @BookingId
    """;

var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(
    sql,
    new { request.BookingId });

return booking; // Implicit conversion to Result<BookingResponse>
```

Source: `src/Bookify.Application/Abstractions/Behaviors/ValidationBehavior.cs`

```csharp
if (validationErrors.Any())
{
    throw new Exceptions.ValidationException(validationErrors); // Mapped by global middleware
}
```

Source: `src/Bookify.Application/Bookings/ReserveBooking/BookingReservedDomainEventHandler.cs`

```csharp
public async Task Handle(BookingReserverdDomainEvent notification, CancellationToken ct)
{
    var booking = await _bookingRepository.GetByIdAsync(notification.bookingId, ct);
    if (booking is null) return;

    var user = await _userRepository.GetByIdAsync(booking.UserId, ct);
    if (user is null) return;

    await _emailService.SendAsync(
        user.Email,
        "Booking Reserved",
        "You have 10 minutes to confirm booking");
}
```

## Flow of Execution
1. API builds command/query object and sends via MediatR.
2. Pipeline behaviors run (logging/validation).
3. Handler executes use-case logic.
4. Handler returns `Result`.
5. API translates result to HTTP response.
6. Related domain events are processed asynchronously via outbox job.

## Common Pitfalls In This Project
- Returning `BadRequest` for every failure without considering semantic status codes.
- Writing SQL query DTOs that accidentally expose domain internals.
- Forgetting validator coverage when adding command fields.
- Catching exceptions and swallowing details (there is one unused catch variable warning).

## Small Exercises / Checkpoints
1. Add a flow chart for `ReserveBookingCommand` from endpoint to DB commit.
2. Compare one command response and one query response contract.
3. Find where validation exceptions become `ProblemDetails` JSON.
4. Identify one place where logging could include correlation ID in the future.
