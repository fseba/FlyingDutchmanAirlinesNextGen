using Moq;

using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

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
      .ReturnsAsync(true);

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Leo Tolstoy"))
      .ReturnsAsync(Customer.Create("Leo Tolstoy"));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);
    
    bool result = await service.CreateBooking("Leo Tolstoy", 0);

    Assert.IsTrue(result);
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  [DataRow("", 0)]
  [DataRow(null, -1)]
  [DataRow("Galileo", -1)]
  public async Task CreateBooking_Failure_InvalidInputArguments(string customerName, int flightNumber)
  {
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    await service.CreateBooking(customerName, flightNumber);
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  public async Task CreateBooking_Failure_RepositoryException_ArgumentException()
  {
    _mockBookingRepository
      .Setup(repository => repository.CreateBooking(0, 1)).Throws(new ArgumentException());
    
    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Galileo Galilei"))
      .ReturnsAsync(Customer.Create("Galileo Galilei"));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(1))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    await service.CreateBooking("Galileo Galilei", 1);
  }

  [TestMethod]
  public async Task CreateBooking_Failure_FlightNotInDatabase()
  {
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);
    bool result = await service.CreateBooking("Maurits Escher", 1);

    Assert.IsFalse(result);
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
  public async Task DeleteBooking_Failure_Returns_False()
  {
    int bookingId = 1;
    _mockBookingRepository
      .Setup(repository => repository.DeleteBooking(bookingId))
      .ReturnsAsync(false);

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object, _mockAirportRepository.Object);

    var result = await service.DeleteBooking(bookingId);

    Assert.IsFalse(result);
  }
}
