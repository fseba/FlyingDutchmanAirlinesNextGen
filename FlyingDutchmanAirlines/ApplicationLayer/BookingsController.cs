using System.Net;
using Microsoft.AspNetCore.Mvc;

using FlyingDutchmanAirlines.ApplicationLayer.JsonData;
using FlyingDutchmanAirlines.BusinessLogicLayer;
using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines.ApplicationLayer;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
  private IBookingService _bookingService;

  public BookingsController(IBookingService bookingService)
  {
    _bookingService = bookingService;
  }

  [HttpPost("{flightNumber}")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> CreateBooking([FromBody] BookingData body, int flightNumber)
  {
    if (flightNumber < 0)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request - Negative flight number");
    }

    try
    {
      var name = $"{body.FirstName} {body.LastName}";

      bool result = await _bookingService.CreateBooking(name, flightNumber);

      return result
        ? StatusCode((int)HttpStatusCode.Created, $"Booking created - Flight {flightNumber} booked for {name}")
        : StatusCode((int)HttpStatusCode.InternalServerError, "The booking creation process encountered an error and was unable to complete");
    }
    catch (ArgumentException ex)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingDTO))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetBookingsByCustomerName([FromBody] BookingData body)
  {
    try
    {
      var customerName = $"{body.FirstName} {body.LastName}";

      Queue<BookingDTO?> bookings = new();
      await foreach (BookingDTO? booking in _bookingService.GetBookingsByCustomerName(customerName))
      {
        bookings.Enqueue(booking);
      }

      return bookings.Count != 0
        ? StatusCode((int)HttpStatusCode.OK, bookings)
        : StatusCode((int)HttpStatusCode.NotFound);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  [HttpGet("{bookingId:int}")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingDTO))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetBookingById(int bookingId)
  {
    if (bookingId < 0)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request - Negative booking id");
    }

    try
    {
      var booking = await _bookingService.GetBookingById(bookingId);

      return booking is not null
        ? StatusCode((int)HttpStatusCode.OK, booking)
        : StatusCode((int)HttpStatusCode.NotFound);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  [HttpDelete("{bookingId:int}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteBooking(int bookingId)
  {
    if (bookingId < 0)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request - Negative booking id");
    }

    try
    {
      var deletedBooking = await _bookingService.DeleteBooking(bookingId);

      return deletedBooking
        ? StatusCode((int)HttpStatusCode.OK, $"Booking {bookingId} successfully deleted")
        : StatusCode((int)HttpStatusCode.NotFound);
    }
    catch (ArgumentException)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad Request");
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }
}

