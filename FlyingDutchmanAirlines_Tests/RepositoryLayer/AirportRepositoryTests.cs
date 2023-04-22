using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class AirportRepositoryTests
{
  private FlyingDutchmanAirlinesContext _context = null!;
  private IAirportRepository _repository = null!;

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
    var airport = await _repository.GetAirportById(airportId);
    Assert.IsNotNull(airport);

    var dbAirport = _context.Airports.First(a => a.AirportId == airportId);

    Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
    Assert.AreEqual(dbAirport.City, airport.City);
    Assert.AreEqual(dbAirport.Iata, airport.Iata);
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  public async Task GetAirportByID_Failure_InvalidInput()
  {
    await _repository.GetAirportById(-1);
  }

  [TestMethod]
  public async Task GetAirportByID_Failure_UnknownID()
  {
    var airport = await _repository.GetAirportById(10);

    Assert.IsNull(airport);
  }
}

