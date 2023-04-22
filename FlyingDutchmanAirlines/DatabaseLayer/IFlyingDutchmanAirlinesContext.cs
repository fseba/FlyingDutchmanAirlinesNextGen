using FlyingDutchmanAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.DatabaseLayer
{
  public interface IFlyingDutchmanAirlinesContext
  {
    DbSet<Airport> Airports { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Flight> Flights { get; set; }
  }
}