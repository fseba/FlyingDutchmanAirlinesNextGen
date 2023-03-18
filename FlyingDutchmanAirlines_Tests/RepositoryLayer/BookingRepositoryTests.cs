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
  public async Task CreateBooking_Failure_InvalidInpute(int customerId, int flightNumber)
  {
    await _repository.CreateBooking(customerId, flightNumber);
  }

  [TestMethod]
  [ExpectedException(typeof(CouldNotAddBookingToDatabaseException))]
  public async Task CreateBooking_Failure_DatabaseError()
  {
    await _repository.CreateBooking(0, 1);
  }

  [TestMethod]
  public async Task DeleteBooking_Success()
  {
    await _repository.CreateBooking(1, 0);
    Booking booking = _context.Bookings.First();

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
  [ExpectedException(typeof(BookingNotFoundException))]
  public async Task DeleteBooking_Failure_BookingNotFound()
  {
    await _repository.DeleteBooking(1);
  }

  [TestMethod]
  public async Task GetBookingById_Success()
  {
    await _repository.CreateBooking(1, 0);
    Booking dbBooking = _context.Bookings.First();

    Booking booking = await _repository.GetBookingById(dbBooking.BookingId);

    Assert.AreSame(dbBooking, booking);
  }

  [TestMethod]
  public async Task GetBookingsByCustomerId_Success()
  {
    await _repository.CreateBooking(1, 0);
    await _repository.CreateBooking(1, 1);
    await _repository.CreateBooking(1, 2);
    int bookingsCount = _context.Bookings.Count();

    Booking[] bookings = await _repository.GetBookingsByCustomerId(1);

    Assert.AreEqual(bookingsCount, bookings.Length);
  }

  [TestMethod]
  [ExpectedException(typeof(BookingNotFoundException))]
  public async Task GetBookings_Failure_NoBookingsInDatabase()
  {
    await _repository.GetBookingsByCustomerId(1);
  }

  [TestMethod]
  [ExpectedException(typeof(BookingNotFoundException))]
  public async Task GetBookingsByCustomerId_Failure_CustomerNotFoundException_Wrong_CustomerId()
  {
    await _repository.CreateBooking(1, 0);
    await _repository.CreateBooking(1, 1);
    await _repository.CreateBooking(1, 2);
    int bookingsCount = _context.Bookings.Count();

    Booking[] bookings = await _repository.GetBookingsByCustomerId(2);
  }
}

