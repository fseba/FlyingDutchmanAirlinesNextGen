using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;

using FlyingDutchmanAirlines.ApplicationLayer;
using FlyingDutchmanAirlines.ApplicationLayer.JsonData;
using FlyingDutchmanAirlines.BusinessLogicLayer;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer;

[TestClass]
public class BookingsControllerTests
{
  private Mock<IBookingService> _mockService = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    _mockService = new();
  }

  [TestMethod]
  public async Task CreateBooking_Success()
  {
    _mockService
      .Setup(service => service.CreateBooking("Bob Bobson", 1))
      .ReturnsAsync(true);

    BookingsController controller = new(_mockService.Object);

    BookingData bookingData = new() { FirstName = "Bob", LastName = "Bobson" };

    ObjectResult? response = await controller.CreateBooking(bookingData, 1) as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.Created, response.StatusCode);
    Assert.AreEqual("Booking created - Flight 1 booked for Bob Bobson", response.Value);
  }

  [TestMethod]
  public async Task CreateBooking_Failure_InternalServerError()
  {
    _mockService
      .Setup(service => service.CreateBooking("Bob Bobson", 1))
      .ReturnsAsync(false);

    BookingsController controller = new(_mockService.Object);

    BookingData bookingData = new() { FirstName = "Bob", LastName = "Bobson" };

    ObjectResult? response = await controller.CreateBooking(bookingData, 1) as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
    Assert.AreEqual("The booking creation process encountered an error and was unable to complete", response.Value);
  }

  [TestMethod]
  public async Task DeleteBooking_Success()
  {
    _mockService
      .Setup(service => service.DeleteBooking(1))
      .ReturnsAsync(true);

    BookingsController controller = new(_mockService.Object);

    ObjectResult? response = await controller.DeleteBooking(1) as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);
    Assert.AreEqual("Booking 1 successfully deleted", response.Value);
  }
}

