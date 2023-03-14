using System;
using Microsoft.EntityFrameworkCore;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class AirportRepositoryTests
{
  private FlyingDutchmanAirlinesContext _context = null!;
  private AirportRepository _repository = null!;

  [TestInitialize]
  public async Task TestInitialize()
  {
    DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
      new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
      .UseInMemoryDatabase("FlyingDutchman").Options;
    _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

    SortedList<string, Airport> airports = new()
    {
      {
        "GOH",
        new Airport
        { AirportId = 0,
              City = "Nuuk",
              Iata = "GOH"
        }
      },
      {
        "PHX",
        new Airport
        {
          AirportId = 1,
          City = "Phoenix",
          Iata = "PHX"
        }
      },
      {
        "DDH",
        new Airport
        {
          AirportId = 2,
          City = "Bennington",
          Iata = "DDH"
        }
      },
      {
       "RDU",
        new Airport
        {
          AirportId = 3,
          City = "Raleigh-Durham",
          Iata = "RDU"
        }
      }
  };

    _context.Airports.AddRange(airports.Values);
    await _context.SaveChangesAsync();

    _repository = new AirportRepository(_context);
    Assert.IsNotNull(_repository);
  }

  [TestMethod]
  [DataRow(0)]
  [DataRow(1)]
  [DataRow(2)]
  [DataRow(3)]
  public async Task GetAirportByID_Success(int airportId)
  {
    Airport airport = await _repository.GetAirportByID(airportId);
    Assert.IsNotNull(airport);

    Airport dbAirport = _context.Airports.First(a => a.AirportId == airportId);

    Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
    Assert.AreEqual(dbAirport.City, airport.City);
    Assert.AreEqual(dbAirport.Iata, airport.Iata);
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  public async Task GetAirportByID_Failure_InvalidInput()
  {
    using StringWriter outputStream = new();

    try
    {
      Console.SetOut(outputStream);
      await _repository.GetAirportByID(-1);
    }
    catch (ArgumentException)
    {
      Assert.IsTrue(outputStream.ToString().Contains("Argument Exception in GetAirpotByID! Airport ID = -1"));
      throw;
    }
   }

  [TestMethod]
  [ExpectedException(typeof(AirportNotFoundException))]
  public async Task GetAirportByID_Failure_UnknownID()
  {
    await _repository.GetAirportByID(10);
  }
}

