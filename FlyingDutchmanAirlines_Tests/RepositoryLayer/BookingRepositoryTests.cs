using System;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;
using Microsoft.EntityFrameworkCore;

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

    _repository = new BookingRepository(_context);
    Assert.IsNotNull(_repository);
  }

  [TestMethod]
  public async Task CreateBooking_Success()
  {
    await _repository.CreateBooking(1, 0);
    Booking booking = _context.Bookings.First();

    Assert.IsNotNull(booking);
    Assert.AreEqual(1, booking.CustomerId);
    Assert.AreEqual(0, booking.FlightNumber);
  }

  [TestMethod]
  [DataRow(-1, 0)]
  [DataRow(0, -1)]
  [DataRow(-1, -1)]
  [ExpectedException(typeof(ArgumentException))]
  public async Task CreateBooking_Failure_InvalidInpute(int customerID, int flightNumber)
  {
    await _repository.CreateBooking(customerID, flightNumber);
  }

  [TestMethod]
  [ExpectedException(typeof(CouldNotAddBookingToDatabaseException))]
  public async Task CreateBooking_Failure_DatabaseError()
  {
    await _repository.CreateBooking(0, 1);
  }
}

