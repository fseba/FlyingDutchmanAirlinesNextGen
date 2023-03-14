using System;
using Microsoft.EntityFrameworkCore;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;



[TestClass]
public class FlightRepositoryTests
{
  private FlyingDutchmanAirlinesContext _context = null!;
  private FlightRepository _repository = null!;

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
      Destination = 2
    };

    Flight flight2 = new()
    {
      FlightNumber = 10,
      Origin = 3,
      Destination = 4
    };

    _context.Flights.Add(flight);
    _context.Flights.Add(flight2);
    await _context.SaveChangesAsync();

    _repository = new FlightRepository(_context);
    Assert.IsNotNull(_repository);
  }

  [TestMethod]
  [ExpectedException(typeof(FlightNotFoundException))]
  public async Task GetFlightByFlightnumber_Failure_InvalidFlightnumber()
  {
    await _repository.GetFlightByFlightNumber(-1);
  }

  [TestMethod]
  public async Task GetFlightByFlightnumber_Success()
  {
    Flight flight = await _repository.GetFlightByFlightNumber(1);
    Assert.IsNotNull(flight);

    Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
    Assert.IsNotNull(dbFlight);

    Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
    Assert.AreEqual(dbFlight.Origin, flight.Origin);
    Assert.AreEqual(dbFlight.Destination, flight.Destination);
  }

  [TestMethod]
  [ExpectedException(typeof(FlightNotFoundException))]
  public async Task GetFlightByFlightNumber_Failure_DatabaseException()
  {
    await _repository.GetFlightByFlightNumber(2);
  }

  [TestMethod]
  public async Task GetFlights_Success()
  {
    Queue<Flight> flights = await _repository.GetFlights();
    Assert.IsNotNull(flights);

    Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
    Assert.IsNotNull(dbFlight);

    Assert.AreEqual(dbFlight.FlightNumber, flights.Peek().FlightNumber);
    Assert.AreEqual(dbFlight.Origin, flights.Peek().Origin);
    Assert.AreEqual(dbFlight.Destination, flights.Peek().Destination);
  }

  [TestMethod]
  [ExpectedException(typeof(FlightNotFoundException))]
  public async Task GetFlights_Failure_NoFlightsInDatabase()
  {
    DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
      new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
      .UseInMemoryDatabase("FlyingDutchman").Options;
    FlyingDutchmanAirlinesContext context = new(dbContextOptions);

    FlightRepository repository = new(context);
    Assert.IsNotNull(repository);

    Queue<Flight> flights = await repository.GetFlights();
  }
}

