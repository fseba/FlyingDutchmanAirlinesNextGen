using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.InfrastuctureLayer;

public class FlightRepository : IFlightRepository
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

  public async Task<Flight?> GetFlightByFlightNumber(int flightNumber)
  {
    if (flightNumber < 0)
    {
      throw new ArgumentException("Invalid flight number - Negative number");
    }

    return await _context.Flights.Include(f => f.DestinationNavigation)
                                 .Include(f => f.OriginNavigation)
                                 .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
  }

  public async Task<Flight[]> GetFlights()
  {
    if (!_context.Flights.Any())
    {
      Console.WriteLine("No flights in database!");
      return Array.Empty<Flight>();
    }

    return await _context.Flights.Include(f => f.DestinationNavigation)
                                 .Include(f => f.OriginNavigation)
                                 .ToArrayAsync();
  }
}

