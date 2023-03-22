﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;

using FlyingDutchmanAirlines.ControllerLayer;
using FlyingDutchmanAirlines.ControllerLayer.JsonData;
using FlyingDutchmanAirlines.ServiceLayer;


namespace FlyingDutchmanAirlines_Tests.ControllerLayer;

[TestClass]
public class BookingsControllerTests
{
  [TestMethod]
  public async Task CreateBooking_Success()
  {
    Mock<BookingService> mockService = new();

    mockService
      .Setup(service => service.CreateBooking("Bob Bobson", 1))
      .ReturnsAsync((true, null));

    BookingsController controller = new(mockService.Object);

    BookingData bookingData = new() { FirstName = "Bob", LastName = "Bobson" };

    ObjectResult? response = await controller.CreateBooking(bookingData, 1) as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.Created, response.StatusCode);
    Assert.AreEqual("Booking created - Flight 1 booked for Bob Bobson", response.Value);
  }

  [TestMethod]
  public async Task CreateBooking_Failure_InternalServerError()
  {
    Mock<BookingService> mockService = new();

    mockService
      .Setup(service => service.CreateBooking("Bob Bobson", 1))
      .ReturnsAsync((false, new ArgumentException()));

    BookingsController controller = new(mockService.Object);

    BookingData bookingData = new() { FirstName = "Bob", LastName = "Bobson" };

    ObjectResult? response = await controller.CreateBooking(bookingData, 1) as ObjectResult;

    Assert.IsNotNull(response);
    Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
    Assert.AreEqual(new ArgumentException().Message, response.Value);
  }
}
