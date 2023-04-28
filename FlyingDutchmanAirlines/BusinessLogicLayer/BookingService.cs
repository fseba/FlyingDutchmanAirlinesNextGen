using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.InfrastuctureLayer.Models;
using FlyingDutchmanAirlines.InfrastuctureLayer;
using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class BookingService : IBookingService
{
  private readonly IBookingRepository _bookingRepository = null!;
  private readonly ICustomerRepository _customerRepository = null!;
  private readonly IFlightRepository _flightRepository = null!;

  public BookingService(ICustomerRepository customerRepository, IBookingRepository bookingRepository, IFlightRepository flightRepository)
  {
    _customerRepository = customerRepository;
    _bookingRepository = bookingRepository;
    _flightRepository = flightRepository;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public BookingService()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new Exception("This constructor should only be used for testing");
    }
  }

  public virtual async Task<bool> CreateBooking(string customerName, int flightNumber)
  {
    if (string.IsNullOrWhiteSpace(customerName) || flightNumber < 0)
    {
      throw new ArgumentException("Invalid flight number or empty username provided");
    }

    if (!await FlightExistsInDatabase(flightNumber))
    {
      return false;
    }

    var customer = await _customerRepository.GetCustomerByName(customerName);

    if (customer is null)
    {
      customer = Customer.Create(customerName) ?? throw new ArgumentException("Could not create customer - Please check customer name");

      bool customerSuccessfullyAdded = await _customerRepository.AddCustomer(customer);

      if (!customerSuccessfullyAdded)
      {
        return false;
      }
    }

    var newBooking = Booking.Create(customer.CustomerId, flightNumber);

    return await _bookingRepository.AddBooking(newBooking);
  }

  public virtual async Task<bool> DeleteBooking(int bookingId)
  {
    if (bookingId < 0)
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    return await _bookingRepository.DeleteBooking(bookingId);
  }

  public virtual async Task<BookingView?> GetBookingById(int bookingId)
  {
    if (bookingId < 0)
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    var booking = await _bookingRepository.GetBookingById(bookingId);

    if (booking is null)
    {
      return null;
    }

    FlightView flightView = new(booking.FlightNumberNavigation);

    return new BookingView(booking, flightView);
  }

  public virtual async IAsyncEnumerable<BookingView?> GetBookingsByCustomerName(string customerName)
  {
    var customer = await _customerRepository.GetCustomerByName(customerName);

    if (customer is not null && customer.Bookings.Any())
    {
      foreach (Booking booking in customer.Bookings)
      {
        yield return await GetBookingById(booking.BookingId);
      }
    }
  }

  private async Task<bool> FlightExistsInDatabase(int flightNumber)
  {
    return await _flightRepository.GetFlightByFlightNumber(flightNumber) != null;
  }
}

