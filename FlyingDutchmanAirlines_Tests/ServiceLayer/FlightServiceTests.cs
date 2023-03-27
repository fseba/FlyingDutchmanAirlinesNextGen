using System;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer;

[TestClass]
public class FlightServiceTests
{
  private Mock<FlightRepository> _mockFlightRepository = null!;
  private Mock<AirportRepository> _mockAirportRepository = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    _mockFlightRepository = new Mock<FlightRepository>();
    _mockAirportRepository = new Mock<AirportRepository>();

    Flight flightInDatabase = new()
    {
      FlightNumber = 148,
      Origin = 31,
      Destination = 92
    };

    Flight[] mockReturn = { flightInDatabase };

    _mockFlightRepository
      .Setup(repository => repository.GetFlights())
      .ReturnsAsync(mockReturn);

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(148))
      .ReturnsAsync(flightInDatabase);

    _mockAirportRepository
      .Setup(repository => repository.GetAirportById(31))
      .ReturnsAsync(new Airport
      {
        AirportId = 31,
        City = "Mexico City",
        Iata = "MEX"
      });

    _mockAirportRepository
      .Setup(repository => repository.GetAirportById(92))
      .ReturnsAsync(new Airport
      {
        AirportId = 92,
        City = "Ulaanbaataar",
        Iata = "UBN"
      });
  }

  [TestMethod]
  public async Task GetFlights_Success()
  {
    FlightService service = new(_mockFlightRepository.Object, _mockAirportRepository.Object);

    await foreach (FlightView flightView in service.GetFlights())
    {
      Assert.IsNotNull(flightView);
      Assert.AreEqual(flightView.FlightNumber, 148);
      Assert.AreEqual(flightView.Origin.City, "Mexico City");
      Assert.AreEqual(flightView.Origin.Code, "MEX");
      Assert.AreEqual(flightView.Destination.City, "Ulaanbaataar");
      Assert.AreEqual(flightView.Destination.Code, "UBN");
    }
  }

  [TestMethod]
  public async Task GetFlights_Failure_Empty_Result()
  {
    _mockFlightRepository
      .Setup(repository => repository.GetFlights())
      .ReturnsAsync(Array.Empty<Flight>());

    FlightService service = new(_mockFlightRepository.Object, _mockAirportRepository.Object);

    List<FlightView> flightViews = new();
    
    await foreach (FlightView flightView in service.GetFlights())
    {
      flightViews.Add(flightView);
    }

    Assert.AreEqual(0, flightViews.Count);
  }

  [TestMethod]
  public async Task GetFlightByFlightNumber_Success()
  {
    FlightService service = new(_mockFlightRepository.Object, _mockAirportRepository.Object);

    var flightView = await service.GetFlightByFlightNumber(148);

    Assert.IsNotNull(flightView);
    Assert.AreEqual(flightView.FlightNumber, 148);
    Assert.AreEqual(flightView.Origin.City, "Mexico City");
    Assert.AreEqual(flightView.Origin.Code, "MEX");
    Assert.AreEqual(flightView.Destination.City, "Ulaanbaataar");
    Assert.AreEqual(flightView.Destination.Code, "UBN");
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  public async Task GetFlightByNumber_Failure_RepositoryException_ArgumentException()
  {
    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(-1))
      .ThrowsAsync(new ArgumentException());

    FlightService service = new(_mockFlightRepository.Object, _mockAirportRepository.Object);

    await service.GetFlightByFlightNumber(-1);
  }

  [TestMethod]
  public async Task GetFlights_Failure_Airport_Null()
  {
    _mockAirportRepository
      .Setup(repository => repository.GetAirportById(31))
      .Returns(Task.FromResult<Airport?>(null));

    _mockAirportRepository
      .Setup(repository => repository.GetAirportById(92))
      .Returns(Task.FromResult<Airport?>(null));

    FlightService service = new(_mockFlightRepository.Object, _mockAirportRepository.Object);

    await foreach (FlightView flightView in service.GetFlights())
    {
      Assert.IsNotNull(flightView);
      Assert.AreEqual(flightView.FlightNumber, 148);
      Assert.AreEqual(flightView.Origin.City, "No City found");
      Assert.AreEqual(flightView.Origin.Code, "No Iata found");
      Assert.AreEqual(flightView.Destination.City, "No City found");
      Assert.AreEqual(flightView.Destination.Code, "No Iata found");
    }
  }
}

