using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.InfrastuctureLayer.Models;
using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines.BusinessLogicLayer;

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

  public async IAsyncEnumerable<FlightDTO> GetFlights()
  {
    IEnumerable<Flight> flights = await _flightRepository.GetFlights();

    foreach (Flight flight in flights)
    {
      yield return new FlightDTO(flight);
    }
  }

  public async Task<FlightDTO?> GetFlightByFlightNumber(int flightNumber)
  {
    var flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);

    if (flight is null)
    {
      return null;
    }

    return new FlightDTO(flight); ;
  }
}

