using System;
using FlyingDutchmanAirlines.InfrastuctureLayer.Models;
using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines_Tests.DTOs;

[TestClass]
public class FlightViewTests
{
  [TestMethod]
  public void Constructor_FlightView_Success()
  {
    Flight flight = new()
    {
      FlightNumber = 0,
      Origin = 31,
      Destination = 92,
      OriginNavigation = new Airport
      {
        AirportId = 31,
        City = "Amsterdam",
        Iata = "AMS"
      },
      DestinationNavigation = new Airport
      {
        AirportId = 92,
        City = "Moscow",
        Iata = "SVO"
      }
    };


    FlightDTO view = new(flight);
    Assert.IsNotNull(view);

    Assert.AreEqual(view.FlightNumber, flight.FlightNumber);
    Assert.AreEqual(view.Origin.City, flight.OriginNavigation.City);
    Assert.AreEqual(view.Origin.Code, flight.OriginNavigation.Iata);
    Assert.AreEqual(view.Destination.City, flight.DestinationNavigation.City);
    Assert.AreEqual(view.Destination.Code, flight.DestinationNavigation.Iata);
  }

  [TestMethod]
  public void Constructor_AirportInfo_Failure_City_EmptyString()
  {
    string destinationCity = string.Empty;
    string destinationCityCode = "SYD";

    AirportInfo airportInfo = new ((destinationCity, destinationCityCode));
    Assert.IsNotNull(airportInfo);

    Assert.AreEqual(airportInfo.City, "No city found");
    Assert.AreEqual(airportInfo.Code, destinationCityCode);
  }

  [TestMethod]
  public void Constructor_AirportInfo_Failure_Code_EmptyString()
  {
    string destinationCity = "Ushuaia";
    string destinationCityCode = string.Empty;

    AirportInfo airportInfo = new ((destinationCity, destinationCityCode));

    Assert.IsNotNull(airportInfo);
    Assert.AreEqual(airportInfo.City, destinationCity);
    Assert.AreEqual(airportInfo.Code, "No code found");
  }
}

