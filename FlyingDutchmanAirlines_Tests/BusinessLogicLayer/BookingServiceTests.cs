using Moq;

using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.BusinessLogicLayer;
using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines_Tests.BusinessLogicLayer;

[TestClass]
public class BookingServiceTests
{
  private Mock<IBookingRepository> _mockBookingRepository = null!;
  private Mock<ICustomerRepository> _mockCustomerRepository = null!;
  private Mock<IFlightRepository> _mockFlightRepository = null!;

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
      .Setup(repository => repository.AddBooking(It.IsAny<Booking>()))
      .ReturnsAsync(true);

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Leo Tolstoy"))
      .ReturnsAsync(Customer.Create("Leo Tolstoy"));

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);
    
    bool result = await service.CreateBooking("Leo Tolstoy", 0);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task CreateBooking_Success_CustomerNotInDatabase()
  {
    _mockBookingRepository
      .Setup(repository => repository.AddBooking(It.IsAny<Booking>()))
      .ReturnsAsync(true);

    _mockCustomerRepository
      .Setup(repository => repository.GetCustomerByName("Konrad Zuse"))
      .Returns(Task.FromResult<Customer?>(null));

    _mockCustomerRepository
      .Setup(repository => repository.AddCustomer(It.IsAny<Customer>()))
      .ReturnsAsync(true);

    _mockFlightRepository
      .Setup(repository => repository.GetFlightByFlightNumber(0))
      .ReturnsAsync(new Flight());

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    bool result = await service.CreateBooking("Konrad Zuse", 0);

    Assert.IsTrue(result);
  }

  [TestMethod]
  [ExpectedException(typeof(ArgumentException))]
  [DataRow("", 0)]
  [DataRow(null, -1)]
  [DataRow("Galileo", -1)]
  public async Task CreateBooking_Failure_InvalidInputArguments(string customerName, int flightNumber)
  {
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    await service.CreateBooking(customerName, flightNumber);
  }

  [TestMethod]
  public async Task CreateBooking_Failure_FlightNotInDatabase()
  {
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);
    bool result = await service.CreateBooking("Maurits Escher", 1);

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task DeleteBooking_Success()
  {
    int bookingId = 1;
    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

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

    BookingService service = new(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockFlightRepository.Object);

    var result = await service.DeleteBooking(bookingId);

    Assert.IsFalse(result);
  }
}
