﻿using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class FlightRepository
{
  private readonly FlyingDutchmanAirlinesContext _context = null!;

  public FlightRepository(FlyingDutchmanAirlinesContext injectedContext)
  {
    _context = injectedContext;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public FlightRepository()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new InvalidOperationException("This constructor should only be used for testing");
    }
  }

  public virtual async Task<Flight?> GetFlightByFlightNumber(int flightNumber)
  {
    if (flightNumber < 0)
    {
      Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! flightNumber = {flightNumber}");
      throw new ArgumentException("Invalid flight number - Negative number");
    }

    return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
  }

  public virtual async Task<Flight[]> GetFlights()
  {
    if (!_context.Flights.Any())
    {
      Console.WriteLine("No flights in database!");
      return Array.Empty<Flight>();
    }

    return await _context.Flights.ToArrayAsync();
  }
}

