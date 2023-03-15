using System;
namespace FlyingDutchmanAirlines.Views;

public class FlightView
{
  public int FlightNumber { get; private set; }
  public AirportInfo Origin { get; private set; }
  public AirportInfo Destination { get; private set; }

  public FlightView(int flightNumber, (string city, string code) origin, (string city, string code) destination)
  {
    FlightNumber = int.IsPositive(flightNumber) ? flightNumber : throw new ArgumentException("Invalid flight number");

    Origin = new AirportInfo(origin);
    Destination = new AirportInfo(destination);
  }
}

public struct AirportInfo
{
  public string City { get; set; }
  public string Code { get; set; }

  public AirportInfo((string city, string code) airport)
  {
    City = string.IsNullOrWhiteSpace(airport.city) ? "No city found" : airport.city;
    Code = string.IsNullOrWhiteSpace(airport.code) ? "No code found" : airport.code;
  }
}

