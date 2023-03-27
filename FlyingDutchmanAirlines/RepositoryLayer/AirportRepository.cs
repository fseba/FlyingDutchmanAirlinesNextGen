using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class AirportRepository
{
  private readonly FlyingDutchmanAirlinesContext _context = null!;

  public AirportRepository(FlyingDutchmanAirlinesContext injectedContext)
  {
    _context = injectedContext;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public AirportRepository()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new InvalidOperationException("This constructor should only be used for testing");
    }
  }

  public virtual async Task<Airport?> GetAirportById(int airportId)
  {
    if (int.IsNegative(airportId))
    {
      Console.WriteLine($"Negativ Id in GetAirpotByID! Airport ID = {airportId}");
      throw new ArgumentException("Invalid AirportId - Negative id");
    }

    return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportId);
  }
}

