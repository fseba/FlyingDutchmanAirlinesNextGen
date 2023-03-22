using System.Net;
using Microsoft.AspNetCore.Mvc;

using FlyingDutchmanAirlines.ControllerLayer.JsonData;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.Views;
using System;

namespace FlyingDutchmanAirlines.ControllerLayer;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
  private BookingService _bookingService;

  public BookingsController(BookingService bookingService)
  {
    _bookingService = bookingService;
  }

  [HttpPost("{flightNumber}")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> CreateBooking([FromBody] BookingData body, int flightNumber)
  {
    if (!ModelState.IsValid)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, ModelState.Root.Errors.First().ErrorMessage);
    }

    string name = $"{body.FirstName} {body.LastName}";
    (bool result, Exception? exception) = await _bookingService.CreateBooking(name, flightNumber);

    return (result && exception == null)
      ? StatusCode((int)HttpStatusCode.Created, $"Booking created - Flight {flightNumber} booked for {name}")
      : StatusCode((int)HttpStatusCode.InternalServerError, exception!.Message);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingView))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetBookingsByCustomerName([FromBody] BookingData body)
  {
    try
    {
      string customerName = $"{body.FirstName} {body.LastName}";

      Queue<BookingView> bookings = new();
      await foreach (BookingView booking in _bookingService.GetBookingsByCustomerName(customerName))
      {
        bookings.Enqueue(booking);
      }

      return StatusCode((int)HttpStatusCode.OK, bookings);
    }
    catch (BookingNotFoundException ex)
    {
      return StatusCode((int)HttpStatusCode.NotFound, $"{ex.Message}");
    }
    catch (Exception)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request");
    }
  }

  [HttpDelete("{bookingId}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteBooking(int bookingId)
  {
    try
    {
      await _bookingService.DeleteBooking(bookingId);

      return StatusCode((int)HttpStatusCode.OK, "Booking successfully deleted");
    }
    catch (BookingNotFoundException)
    {
      return StatusCode((int)HttpStatusCode.NotFound, "No bookings were found in the database");
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

