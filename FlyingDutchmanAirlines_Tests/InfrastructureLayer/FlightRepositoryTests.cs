using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines_Tests.InfrastructureLayer;

[TestClass]
public class FlightRepositoryTests
{
  private FlyingDutchmanAirlinesContext _context = null!;
  private IFlightRepository _repository = null!;

  [TestInitialize]
  public async Task TestInitialize()
  {
    DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
      new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
      .UseInMemoryDatabase("FlyingDutchman").Options;
    _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

    Flight flight = new()
    {
      FlightNumber = 1,
      Origin = 1,
      Destination = 2,
      OriginNavigation = new Airport
      {
        AirportId = 1,
        City = "Mexico City",
        Iata = "MEX"
      },
      DestinationNavigation = new Airport
      {
        AirportId = 2,
        City = "Ulaanbaataar",
        Iata = "UBN"
      }
    };

    Flight flight2 = new()
    {
      FlightNumber = 10,
      Origin = 3,
      Destination = 4,
      OriginNavigation = new Airport
      {
        AirportId = 3,
        City = "Mexico City",
        Iata = "MEX"
      },
      DestinationNavigation = new Airport
      {
        AirportId = 4,
        City = "Ulaanbaataar",
        Iata = "UBN"
      }
    };

    _context.Flights.Add(flight);
    _context.Flights.Add(flight2);
    await _context.SaveChangesAsync();

    _repository = new FlightRepository(_context);
    Assert.IsNotNull(_repository);
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  public async Task GetFlightByFlightnumber_Failure_InvalidFlightnumber_ArgumentException()
  {
    await _repository.GetFlightByFlightNumber(-1);
  }

  [TestMethod]
  public async Task GetFlightByFlightnumber_Success()
  {
    var flight = await _repository.GetFlightByFlightNumber(1);
    Assert.IsNotNull(flight);

    var dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
    Assert.IsNotNull(dbFlight);

    Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
    Assert.AreEqual(dbFlight.Origin, flight.Origin);
    Assert.AreEqual(dbFlight.Destination, flight.Destination);
  }

  [TestMethod]
  public async Task GetFlightByFlightNumber_Failure_UnknownId_Returns_Null()
  {
    var flight = await _repository.GetFlightByFlightNumber(2);

    Assert.IsNull(flight);
  }

  [TestMethod]
  public async Task GetFlights_Success()
  {
    Flight[] flights = await _repository.GetFlights();
    Assert.IsNotNull(flights);

    var dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
    Assert.IsNotNull(dbFlight);

    Assert.AreEqual(dbFlight.FlightNumber, flights.First().FlightNumber);
    Assert.AreEqual(dbFlight.Origin, flights.First().Origin);
    Assert.AreEqual(dbFlight.Destination, flights.First().Destination);
  }

  [TestMethod]
  public async Task GetFlights_Failure_NoFlightsInDatabase()
  {
    DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
      new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
      .UseInMemoryDatabase("FlyingDutchman").Options;
    FlyingDutchmanAirlinesContext context = new(dbContextOptions);

    FlightRepository repository = new(context);
    Assert.IsNotNull(repository);

    Flight[] flights = await repository.GetFlights();

    Assert.AreEqual(0, flights.Length);
  }
}

