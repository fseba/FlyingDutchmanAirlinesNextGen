﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

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
      throw new Exception("This constructor should only be used for testing");
    }
  }

  public virtual async Task<Airport> GetAirportByID(int airportID)
  {
    if (!airportID.IsPositive())
    {
      Console.WriteLine($"Argument Exception in GetAirpotByID! Airport ID = {airportID}");
      throw new ArgumentException("Invalid argument provided");
    }

    return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportID)
      ?? throw new AirportNotFoundException();
  }
}

