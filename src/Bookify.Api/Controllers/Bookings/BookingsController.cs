using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookify.Api.Controllers.Bookings;

[ApiController]
[Route("api/bookings_c")]
public class BookingsController : ControllerBase {
    private readonly ISender _sender;

    public BookingsController(ISender sender) {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(Guid id, CancellationToken cancellationToken) {
        var query = new GetBookingQuery(id);

        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ReserveBooking(
        ReserveBookingRequest request,
        CancellationToken cancellationToken) {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        var command = new ReserveBookingCommand(
            request.ApartmentId,
            Guid.Parse(userId!),
            request.StartDate,
            request.EndDate);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetBooking), new { id = result.Value }, result.Value);
    }
}
