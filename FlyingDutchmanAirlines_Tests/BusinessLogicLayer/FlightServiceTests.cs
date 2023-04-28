using Moq;

using FlyingDutchmanAirlines.InfrastuctureLayer.Models;
using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.BusinessLogicLayer;
using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines_Tests.BusinessLogicLayer;

[TestClass]
public class FlightServiceTests
{
  private Mock<IFlightRepository> _mockFlightRepository = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    _mockFlightRepository = new();

    Flight flightInDatabase = new()
    {
      FlightNumber = 148,
      Origin = 31,
      Destination = 92,
      OriginNavigation = new Airport
      {
        AirportId = 31,
        City = "Mexico City",
        Iata = "MEX"
      },
      DestinationNavigation = new Airport
      {
        AirportId = 92,
        City = "Ulaanbaataar",
        Iata = "UBN"
      }
    };

    Flight[] mockReturn = { flightInDatabase };

    _mockFlightRepository
      .Setup(repository => repository.GetFlights())
      .ReturnsAsync(mockReturn);

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(148))
      .ReturnsAsync(flightInDatabase);
  }

  [TestMethod]
  public async Task GetFlights_Success()
  {
    FlightService service = new(_mockFlightRepository.Object);

    await foreach (FlightDTO flightView in service.GetFlights())
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

    FlightService service = new(_mockFlightRepository.Object);

    List<FlightDTO> flightViews = new();
    
    await foreach (FlightDTO flightView in service.GetFlights())
    {
      flightViews.Add(flightView);
    }

    Assert.AreEqual(0, flightViews.Count);
  }

  [TestMethod]
  public async Task GetFlightByFlightNumber_Success()
  {
    FlightService service = new(_mockFlightRepository.Object);

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

    FlightService service = new(_mockFlightRepository.Object);

    await service.GetFlightByFlightNumber(-1);
  }
}

