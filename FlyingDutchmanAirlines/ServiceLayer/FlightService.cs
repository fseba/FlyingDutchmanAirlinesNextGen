using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class FlightService
{
  private readonly FlightRepository _flightRepository = null!;
  private readonly AirportRepository _airportRepository = null!;

  public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
  {
    _flightRepository = flightRepository;
    _airportRepository = airportRepository;
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
      var originAirport = await GetAirportOrDefault(flight.Origin);
      var destinationAirport = await GetAirportOrDefault(flight.Destination);


      yield return new FlightView(flight.FlightNumber,
                                 (originAirport!.City, originAirport.Iata),
                                 (destinationAirport!.City, destinationAirport.Iata));
    }
  }

  public virtual async Task<FlightView?> GetFlightByFlightNumber(int flightNumber)
  {
    var flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);

    if (flight is null)
    {
      return null;
    }

    var originAirport = await GetAirportOrDefault(flight.Origin);
    var destinationAirport = await GetAirportOrDefault(flight.Destination);

    return new FlightView(flight.FlightNumber,
                         (originAirport!.City, originAirport.Iata),
                         (destinationAirport!.City, destinationAirport.Iata));
  }

  private async Task<Airport> GetAirportOrDefault(int airportNumber)
  {
    return await _airportRepository.GetAirportById(airportNumber) ?? new Airport() { City = "No City found", Iata = "No Iata found" };
  }

}

