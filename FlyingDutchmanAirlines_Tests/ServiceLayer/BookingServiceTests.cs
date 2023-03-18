using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer;

[TestClass]
public class BookingServiceTests
{
  private Mock<BookingRepository> _mockBookingRepository = null!;
  private Mock<CustomerRepository> _mockCustomerRepository = null!;
  private Mock<FlightRepository> _mockFlightRepository = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    _mockBookingRepository = new();
    _mockCustomerRepository = new();
    _mockFlightRepository = new();
  }


  [TestMethod]
  public async Task CreateBooking_Success()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(0, 0)).Returns(Task.CompletedTask);

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Leo Tolstoy"))
      .Returns(Task.FromResult(new Customer("Leo Tolstoy")));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);
    
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
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

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
      .Returns(Task.FromResult(new Customer("Galileo Galilei") { CustomerId = 0 }));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(1))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

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
      .Returns(Task.FromResult(new Customer("Eise Eisingy") { CustomerId = 1 }));

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking("Eise Eisingy", 2);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
  }

  [TestMethod]
  public async Task CreateBooking_Failure_FlightNotInDatabase()
  {
    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(1))
      .Throws(new FlightNotFoundException());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);
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
      .Throws(new CustomerNotFoundException())
      .ReturnsAsync(new Customer("Konrad Zuse"));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

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

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    (bool result, Exception? exception) = await service.CreateBooking("Bill Gates", 0);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
  }

  [TestMethod]
  public async Task DeleteBooking_Success()
  {
    _mockBookingRepository
      .Setup(repository => repository.DeleteBooking(1))
      .Returns(Task.CompletedTask);

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    (bool result, Exception? exception) = await service.DeleteBooking(1);

    Assert.IsTrue(result);
    Assert.IsNull(exception);
  }

  [TestMethod]
  public async Task DeleteBooking_Failure_BookingNotFoundException()
  {
    _mockBookingRepository
      .Setup(repository => repository.DeleteBooking(1))
      .ThrowsAsync(new BookingNotFoundException());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    (bool result, Exception? exception) = await service.DeleteBooking(1);

    Assert.IsFalse(result);
    Assert.IsNotNull(exception);
    Assert.IsInstanceOfType(exception, typeof(BookingNotFoundException));
  }
}
