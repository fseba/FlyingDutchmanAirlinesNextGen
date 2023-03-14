using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.ControllerLayer;

[Route("[controller]")]
public class FlightController : ControllerBase
{
  private readonly FlightService _service;

  public FlightController(FlightService service)
  {
    _service = service;
  }

  [HttpGet]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Queue<FlightView>))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetFlights()
  {
    try
    {
      Queue<FlightView> flights = new();
      await foreach (FlightView flight in _service.GetFlights())
      {
        flights.Enqueue(flight);
      }

      return StatusCode((int)HttpStatusCode.OK, flights);
    }
    catch (FlightNotFoundException)
    {
      return StatusCode((int)HttpStatusCode.NotFound, "No flights were found in the database");
    }
    catch (Exception)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred");
    }
  }

  [HttpGet("{flightNumber}")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlightView))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetFlightByFlightNumber(int flightNumber)
  {
    try
    {
      if (!flightNumber.IsPositive())
      {
        throw new Exception();
      }

      FlightView flight = await _service.GetFlightByFlightNumber(flightNumber);
      return StatusCode((int)HttpStatusCode.OK, flight);
    }
    catch (FlightNotFoundException)
    {
      return StatusCode((int)HttpStatusCode.NotFound, "The flight was not found in the database");
    }
    catch (Exception)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request");
    }
  }
}

