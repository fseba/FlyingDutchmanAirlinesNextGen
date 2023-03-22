using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

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
      throw new Exception("This constructor should only be used for testing");
    }
  }

  public virtual async Task<Flight> GetFlightByFlightNumber(int flightNumber)
  {
    if (int.IsNegative(flightNumber))
    {
      Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! flightNumber = {flightNumber}");
      throw new FlightNotFoundException();
    }

    return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber)
      ?? throw new FlightNotFoundException();
  }

  public virtual async Task<Flight[]> GetFlights()
  {
    if (!_context.Flights.Any())
    {
      Console.WriteLine("No flights in database!");
      throw new FlightNotFoundException();
    }

    return await _context.Flights.ToArrayAsync();
  }
}

