using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines.BusinessLogicLayer;

public interface IFlightService
{
  Task<FlightDTO?> GetFlightByFlightNumber(int flightNumber);
  IAsyncEnumerable<FlightDTO> GetFlights();
}