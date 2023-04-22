using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class FlightService : IFlightService
{
  private readonly IFlightRepository _flightRepository = null!;

  public FlightService(IFlightRepository flightRepository)
  {
    _flightRepository = flightRepository;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public FlightService()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new Exception("This constructor should only be used for testing");
    }
  }

  public virtual async IAsyncEnumerable<FlightView> GetFlights()
  {
    IEnumerable<Flight> flights = await _flightRepository.GetFlights();

    foreach (Flight flight in flights)
    {
      yield return new FlightView(flight.FlightNumber,
                                 (flight.OriginNavigation.City, flight.OriginNavigation.Iata),
                                 (flight.DestinationNavigation.City, flight.DestinationNavigation.Iata));
    }
  }

  public virtual async Task<FlightView?> GetFlightByFlightNumber(int flightNumber)
  {
    var flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);

    if (flight is null)
    {
      return null;
    }

    return new FlightView(flight.FlightNumber,
                         (flight.OriginNavigation.City, flight.OriginNavigation.Iata),
                         (flight.DestinationNavigation.City, flight.DestinationNavigation.Iata)); ;
  }
}

