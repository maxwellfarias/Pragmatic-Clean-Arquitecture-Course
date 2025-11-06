using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookify.Api.Controllers.Bookings;


public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder Group) {

        var builder = Group
            .MapGroup("api/bookings")
            .RequireAuthorization();

        builder.MapGet("{id}", GetBooking);
        builder.MapPost("", ReserveBooking);

        return Group;
    }



    [EndpointName("GetBooking")]
    public static async Task<Results<Ok<BookingResponse>,NotFound>> 
        GetBooking(Guid id, ISender sender, CancellationToken ct)
    {
        var query = new GetBookingQuery(id);

        var result = await sender.Send(query, ct);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
    }

    public static async Task<Results<CreatedAtRoute<Guid>, BadRequest<Error>>> 
        ReserveBooking(HttpContext httpContext,
        ReserveBookingRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new ReserveBookingCommand(
            request.ApartmentId,
            Guid.Parse(userId!),
            request.StartDate,
            request.EndDate);

        var result = await sender.Send(command, ct);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.CreatedAtRoute(result.Value, nameof(GetBooking),new { id = result.Value });
    }
}
