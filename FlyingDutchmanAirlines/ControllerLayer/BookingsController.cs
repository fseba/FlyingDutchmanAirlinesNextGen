using System.Net;
using Microsoft.AspNetCore.Mvc;

using FlyingDutchmanAirlines.ControllerLayer.JsonData;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using System;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

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
    if (int.IsNegative(flightNumber))
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
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingView))]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetBookingsByCustomerName([FromBody] BookingData body)
  {
    try
    {
      var customerName = $"{body.FirstName} {body.LastName}";

      var bookings = new Queue<BookingView?>();
      await foreach (BookingView? booking in _bookingService.GetBookingsByCustomerName(customerName))
      {
        bookings.Enqueue(booking);
      }

      return bookings.Count != 0
        ? StatusCode((int)HttpStatusCode.OK, bookings)
        : StatusCode((int)HttpStatusCode.NoContent);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  [HttpGet("{bookingId:int}")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingView))]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetBookingById(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request - Negative booking id");
    }

    try
    {
      var booking = await _bookingService.GetBookingById(bookingId);

      return StatusCode((int)HttpStatusCode.OK, booking);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  [HttpDelete("{bookingId:int}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteBooking(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request - Negative booking id");
    }

    try
    {
      var deletedBooking = await _bookingService.DeleteBooking(bookingId);

      return deletedBooking is not null
        ? StatusCode((int)HttpStatusCode.OK, $"Booking {deletedBooking.BookingId} for {deletedBooking.Customer!.Name} successfully deleted")
        : StatusCode((int)HttpStatusCode.NoContent);
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

