using System;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines_Tests.Views;

[TestClass]
public class FlightViewTests
{
  [TestMethod]
  public void Constructor_FlightView_Success()
  {
    int flightNumber = 0;
    string originCity = "Amsterdam";
    string originCityCode = "AMS";
    string destinationCity = "Moscow";
    string destinationCityCode = "SVO";

    FlightView view = new(flightNumber, (originCity, originCityCode), (destinationCity, destinationCityCode));
    Assert.IsNotNull(view);

    Assert.AreEqual(view.FlightNumber, flightNumber);
    Assert.AreEqual(view.Origin.City, originCity);
    Assert.AreEqual(view.Origin.Code, originCityCode);
    Assert.AreEqual(view.Destination.City, destinationCity);
    Assert.AreEqual(view.Destination.Code, destinationCityCode);
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

