using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using FlyingDutchmanAirlines.ControllerLayer.JsonData;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ControllerLayer;

[Route("[controller]")]
[Produces("application/json")]
public class BookingController : ControllerBase
{
  private BookingService _bookingService;

  public BookingController(BookingService bookingService)
  {
    _bookingService = bookingService;
  }

  [HttpPost("{flightNumber}")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> CreateBooking([FromBody] BookingData body, int flightNumber)
  {
    if (!ModelState.IsValid)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ModelState.Root.Errors.First().ErrorMessage);
    }

    string name = $"{body.FirstName} {body.LastName}";
    (bool result, Exception? exception) = await _bookingService.CreateBooking(name, flightNumber);

    return (result && exception == null)
      ? StatusCode((int)HttpStatusCode.Created)
      : StatusCode((int)HttpStatusCode.InternalServerError, exception!.Message);
  }
}

