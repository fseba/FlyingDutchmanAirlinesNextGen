﻿using System.Net;
using Microsoft.AspNetCore.Mvc;

using FlyingDutchmanAirlines.BusinessLogicLayer;
using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines.ApplicationLayer;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class FlightsController : ControllerBase
{
  private readonly IFlightService _flightService;

  public FlightsController(IFlightService service)
  {
    _flightService = service;
  }

  [HttpGet]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Queue<FlightDTO>))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetFlights()
  {
    try
    {
      Queue<FlightDTO> flights = new();
      await foreach (FlightDTO flight in _flightService.GetFlights())
      {
        flights.Enqueue(flight);
      }

      return flights.Count != 0
        ? StatusCode((int)HttpStatusCode.OK, flights)
        : StatusCode((int)HttpStatusCode.NotFound);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  [HttpGet("{flightNumber}")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlightDTO))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetFlightByFlightNumber(int flightNumber)
  {
    if (flightNumber < 0)
    {
      return StatusCode((int)HttpStatusCode.BadRequest, "Bad request - Negative flight number");
    }

    try
    {
      var flight = await _flightService.GetFlightByFlightNumber(flightNumber);

      return flight is not null
        ? StatusCode((int)HttpStatusCode.OK, flight)
        : StatusCode((int)HttpStatusCode.NotFound);
    }
    catch (Exception ex)
    {
      return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    }
  }
}

