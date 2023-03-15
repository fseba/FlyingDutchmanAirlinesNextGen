using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
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
    Queue<Flight> flights = await _flightRepository.GetFlights();

    foreach (Flight flight in flights)
    {
      Airport originAirport;
      Airport destinationAirport;

      try
      {
        originAirport = await _airportRepository.GetAirportByID(flight.Origin);
        destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);
      }
      catch (FlightNotFoundException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new ArgumentException($"{ex.Message}", ex.InnerException);
      }

      yield return new FlightView(flight.FlightNumber,
                                 (originAirport.City, originAirport.Iata),
                                 (destinationAirport.City, destinationAirport.Iata));
    }
  }

  public virtual async Task<FlightView> GetFlightByFlightNumber(int flightNumber)
  {
    try
    {
      Flight flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);
      Airport originAirport = await _airportRepository.GetAirportByID(flight.Origin);
      Airport destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);

      return new FlightView
        (flight.FlightNumber,
        (originAirport.City, originAirport.Iata),
        (destinationAirport.City, destinationAirport.Iata));
    }
    catch (FlightNotFoundException)
    {
      throw;
    }
    catch (Exception ex)
    {
      throw new ArgumentException($"{ex.Message}", ex.InnerException);
    }
  }
}

