using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class BookingRepositoryTests
{
  private FlyingDutchmanAirlinesContext _context = null!;
  private BookingRepository _repository = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
      new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
      .UseInMemoryDatabase("FlyingDutchman").Options;
    _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

    Flight flight = new()
    {
      FlightNumber = 0,
      Origin = 1,
      Destination = 2,
      OriginNavigation = new Airport
      {
        AirportId = 1,
        City = "Mexico City",
        Iata = "MEX"
      },
      DestinationNavigation = new Airport
      {
        AirportId = 2,
        City = "Ulaanbaataar",
        Iata = "UBN"
      }
    };

    _context.Flights.Add(flight);
    _context.SaveChangesAsync();

    _repository = new BookingRepository(_context);
    Assert.IsNotNull(_repository);
  }

  [TestMethod]
  public async Task CreateBooking_Success()
  {
    await _repository.CreateBooking(1, 0);
    var booking = _context.Bookings.First();

    Assert.IsNotNull(booking);
    Assert.AreEqual(1, booking.CustomerId);
    Assert.AreEqual(0, booking.FlightNumber);
  }

  [TestMethod]
  [DataRow(-1, 0)]
  [DataRow(0, -1)]
  [DataRow(-1, -1)]
  [ExpectedException(typeof(ArgumentException))]
  public async Task CreateBooking_Failure_InvalidInpute(int customerId, int flightNumber)
  {
    await _repository.CreateBooking(customerId, flightNumber);
  }

  [TestMethod]
  public async Task DeleteBooking_Success()
  {
    await _repository.CreateBooking(1, 0);
    var booking = _context.Bookings.First();

    await _repository.DeleteBooking(booking.BookingId);

    Assert.AreEqual(0, _context.Bookings.Count());
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  public async Task DeleteBooking_Failure_NegativeBookingNumber()
  {
    await _repository.DeleteBooking(-1);
  }

  [TestMethod]
  public async Task DeleteBooking_Failure_Returns_False()
  {
    var result = await _repository.DeleteBooking(1);

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task GetBookingById_Success()
  {
    await _repository.CreateBooking(1, 0);
    var dbBooking = _context.Bookings.First();

    var booking = await _repository.GetBookingById(dbBooking.BookingId);

    Assert.AreSame(dbBooking, booking);
  }

  [TestMethod]
  public async Task GetBookingById_Failure_NoBookingsInDatabase()
  {
    var booking = await _repository.GetBookingById(1);

    Assert.IsNull(booking);
  }

  [TestMethod]
  public async Task GetBookingById_Failure_Wrong_Id()
  {
    await _repository.CreateBooking(1, 0);

    var booking = await _repository.GetBookingById(2);

    Assert.IsNull(booking);
  }
}

