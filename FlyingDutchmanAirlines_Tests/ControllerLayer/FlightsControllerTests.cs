using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;

using FlyingDutchmanAirlines.ControllerLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer;

[TestClass]
public class FlightsControllerTests
{
  private Mock<IFlightService> _mockService = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    _mockService = new();
  }

    [TestMethod]
  public async Task GetFlights_Success_200()
  {
    List<FlightView> returnFlightViews = new(2)
    {
      new FlightView(new Flight()
      {
        FlightNumber = 1932,
        OriginNavigation = new Airport
          {
            AirportId = 31,
            City = "Groningen",
            Iata = "GRQ"
          },
        DestinationNavigation = new Airport
          {
            AirportId = 92,
            City = "Phoenix",
            Iata = "PHX"
          }
      }),
      new FlightView(new Flight()
      {
        FlightNumber = 841,
        OriginNavigation = new Airport
        {
          AirportId = 31,
          City = "New York City",
          Iata = "JFK"
        },
        DestinationNavigation = new Airport
        {
          AirportId = 92,
          City = "London",
          Iata = "LHR"
        }
      })
    };

    _mockService
      .Setup(s => s.GetFlights())
      .Returns(FlightViewAsyncGenerator(returnFlightViews));

    FlightsController controller = new(_mockService.Object);

    ObjectResult? response = await controller.GetFlights() as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

    Queue<FlightView>? content = response.Value as Queue<FlightView>;
    Assert.IsNotNull(content);

    Assert.IsTrue(returnFlightViews.All(flight => content.Contains(flight)));
  }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
  private async IAsyncEnumerable<FlightView> FlightViewAsyncGenerator(IEnumerable<FlightView> views)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
  {
    foreach (FlightView flightView in views)
    {
      yield return flightView;
    }
  }

  [TestMethod]
  public async Task GetFlight_Failure_Empty_Result_404()
  {
    var emptyFlightViews = Enumerable.Empty<FlightView>();

    _mockService
      .Setup(s => s.GetFlights())
      .Returns(FlightViewAsyncGenerator(emptyFlightViews));

    FlightsController controller = new(_mockService.Object);

    StatusCodeResult? response = await controller.GetFlights() as StatusCodeResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
  }

  [TestMethod]
  public async Task GetFlight_Failure_ArugumentException_500()
  {
    _mockService
      .Setup(s => s.GetFlights())
      .Throws(new ArgumentException());

    FlightsController controller = new(_mockService.Object);

    ObjectResult? response = await controller.GetFlights() as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
  }

  [TestMethod]
  public async Task GetFlightByFlightNumber_Success_200()
  {
    FlightView returnedFlightView = new(new Flight()
    {
      FlightNumber = 0,
      OriginNavigation = new Airport
      {
        AirportId = 31,
        City = "Lagos",
        Iata = "LOS"
      },
      DestinationNavigation = new Airport
      {
        AirportId = 92,
        City = "Marrakesh",
        Iata = "RAK"
      }
    });

    _mockService
      .Setup(s => s.GetFlightByFlightNumber(0))
      .ReturnsAsync(returnedFlightView);

    FlightsController controller = new(_mockService.Object);

    ObjectResult? response = await controller.GetFlightByFlightNumber(0) as ObjectResult;
    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

    FlightView? content = response.Value as FlightView;
    Assert.IsNotNull(content);

    Assert.AreEqual(returnedFlightView, content);
  }

  [TestMethod]
  public async Task GetFlightByFlightNumber_Failure_Empty_Result_404()
  {
    _mockService
      .Setup(s => s.GetFlightByFlightNumber(0))
      .Returns(Task.FromResult<FlightView?>(null));

    FlightsController controller = new(_mockService.Object);

    StatusCodeResult? response = await controller.GetFlightByFlightNumber(0) as StatusCodeResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
  }

  [TestMethod]
  public async Task GetFlightByFlightNumber_Failure_ArugumentException_400()
  {
    FlightsController controller = new(_mockService.Object);

    ObjectResult? response = await controller.GetFlightByFlightNumber(-1) as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    Assert.AreEqual("Bad request - Negative flight number", response.Value);
  }
}

