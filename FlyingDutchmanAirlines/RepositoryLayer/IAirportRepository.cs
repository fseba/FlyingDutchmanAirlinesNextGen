using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
  public interface IAirportRepository
  {
    Task<Airport?> GetAirportById(int airportId);
  }
}