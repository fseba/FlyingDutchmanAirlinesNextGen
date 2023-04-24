using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.Views;

public class FlightView
{
  public int FlightNumber { get; private set; }
  public AirportInfo Origin { get; private set; }
  public AirportInfo Destination { get; private set; }

  public FlightView(Flight flight)
  {
    if (flight is null)
    {
      throw new ArgumentNullException(nameof(flight));
    }

    FlightNumber = flight.FlightNumber;
    Origin = new AirportInfo((flight.OriginNavigation.City, flight.OriginNavigation.Iata));
    Destination = new AirportInfo((flight.DestinationNavigation.City, flight.DestinationNavigation.Iata));
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

