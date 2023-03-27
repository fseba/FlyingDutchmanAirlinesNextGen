using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer;

[TestClass]
public class BookingServiceTests
{
  private Mock<BookingRepository> _mockBookingRepository = null!;
  private Mock<CustomerRepository> _mockCustomerRepository = null!;
  private Mock<FlightRepository> _mockFlightRepository = null!;
  private Mock<AirportRepository> _mockAirportRepository = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    _mockBookingRepository = new();
    _mockCustomerRepository = new();
    _mockFlightRepository = new();
    _mockAirportRepository = new();
  }


  [TestMethod]
  public async Task CreateBooking_Success()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(0, 0))
      .Returns(Task.CompletedTask);

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Leo Tolstoy"))
      .ReturnsAsync(new Customer("Leo Tolstoy"));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);
    
    (bool result, Exception? exception) = await service.CreateBooking("Leo Tolstoy", 0);

    Assert.IsTrue(result);
    Assert.IsNull(exception);
  }

  [TestMethod]
  [DataRow("", 0)]
  [DataRow(null, -1)]
  [DataRow("Galileo", -1)]
  public async Task CreateBooking_Failure_InvalidInputArguments(string customerName, int flightNumber)
  {
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking(customerName, flightNumber);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
  }

  [TestMethod]
  public async Task CreateBooking_Failure_RepositoryException_ArgumentException()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(0, 1)).Throws(new ArgumentException());
    
    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Galileo Galilei"))
      .ReturnsAsync(new Customer("Galileo Galilei") { CustomerId = 0 });

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(1))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking("Galileo Galilei", 1);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(ArgumentException));
  }

  [TestMethod]
  public async Task CreateBooking_Failure_RepositoryException_CouldNotAddBookingToDatabase()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(1, 2)).Throws(new CouldNotAddBookingToDatabaseException());

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Eise Eisingy"))
      .ReturnsAsync(new Customer("Eise Eisingy") { CustomerId = 1 });

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking("Eise Eisingy", 2);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
  }

  [TestMethod]
  public async Task CreateBooking_Failure_FlightNotInDatabase()
  {
    //_mockFlightRepository
    //  .Setup(repository => repository.GetFlightByFlightNumber(1))
    //  .Throws(new FlightNotFoundException());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);
    (bool result, Exception? exception) = await service.CreateBooking("Maurits Escher", 1);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
  }

  [TestMethod]
  public async Task CreateBooking_Success_CustomerNotInDatabase()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(0, 0))
      .Returns(Task.CompletedTask);

    _mockCustomerRepository
      .SetupSequence(repository => repository.GetCustomerByName("Konrad Zuse"))
      .Returns(Task.FromResult<Customer?>(null))
      .ReturnsAsync(new Customer("Konrad Zuse"));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking("Konrad Zuse", 0);

    Assert.IsTrue(result);
    Assert.IsNull(exception);
  }

  [TestMethod]
  public async Task CreateBooking_Failure_CustomerNotInDatabase_RepositoryFailure()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(0, 0))
      .Throws(new CouldNotAddBookingToDatabaseException());

    _mockFlightRepository
     .Setup(repository => repository.GetFlightByFlightNumber(0))
     .ReturnsAsync(new Flight());

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Bill Gates"))
      .ReturnsAsync(new Customer("Bill Gates"));

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking("Bill Gates", 0);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
  }

  [TestMethod]
  public async Task DeleteBooking_Success()
  {
    int bookingId = 1;
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    await service.DeleteBooking(bookingId);

    _mockBookingRepository.Verify(r => r.DeleteBooking(bookingId), Times.Once());
  }

  [TestMethod]
  public async Task DeleteBooking_Failure_Returns_Null()
  {
    int bookingId = 1;
    _mockBookingRepository
      .Setup(repository => repository.DeleteBooking(bookingId))
      .Returns(Task.FromResult<Booking?>(null));

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    var deletedBooking = await service.DeleteBooking(bookingId);

    Assert.IsNull(deletedBooking);
  }

  [TestMethod]
  public async Task GetBookingById_Success()
  {
    _mockBookingRepository
      .Setup(repository => repository.GetBookingById(1))
      .ReturnsAsync(new Booking
      {
        BookingId = 1,
        Customer = new Customer("Bob Bobson") { CustomerId = 1 },
        CustomerId = 1,
        FlightNumber = 148
      });

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(148))
      .ReturnsAsync(new Flight
      {
        FlightNumber = 148,
        Origin = 31,
        Destination = 92
      });

    _mockAirportRepository
      .Setup(repository => repository.GetAirportByID(31))
      .ReturnsAsync(new Airport
      {
        AirportId = 31,
        City = "Mexico City",
        Iata = "MEX"
      });

    _mockAirportRepository
      .Setup(repository => repository.GetAirportByID(92))
      .ReturnsAsync(new Airport
      {
        AirportId = 92,
        City = "Ulaanbaataar",
        Iata = "UBN"
      });

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    var bookingView = await service.GetBookingById(1);

    Assert.IsNotNull(bookingView);
    Assert.AreEqual(bookingView.BookingId, 1);
    Assert.AreEqual(bookingView.CustomerId, 1);
    Assert.AreEqual(bookingView.CustomerName, "Bob Bobson");
    Assert.AreEqual(bookingView.FlightView.FlightNumber, 148);
    Assert.AreEqual(bookingView.FlightView.Origin.City, "Mexico City");
    Assert.AreEqual(bookingView.FlightView.Origin.Code, "MEX");
    Assert.AreEqual(bookingView.FlightView.Destination.City, "Ulaanbaataar");
    Assert.AreEqual(bookingView.FlightView.Destination.Code, "UBN");
  }
}
