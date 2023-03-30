using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class BookingService
{
  private readonly BookingRepository _bookingRepository = null!;
  private readonly CustomerRepository _customerRepository = null!;
  private readonly FlightRepository _flightRepository = null!;
  private readonly AirportRepository _airportRepository = null!;

  public BookingService(CustomerRepository customerRepository, BookingRepository bookingRepository, FlightRepository flightRepository, AirportRepository airportRepository)
  {
    _customerRepository = customerRepository;
    _bookingRepository = bookingRepository;
    _flightRepository = flightRepository;
    _airportRepository = airportRepository;
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
    if (string.IsNullOrWhiteSpace(customerName) || int.IsNegative(flightNumber))
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

      if (!customerSuccessfullyAdded) return false;
    }

    return await _bookingRepository.CreateBooking(customer.CustomerId, flightNumber);
  }

  public virtual async Task<bool> DeleteBooking(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    return await _bookingRepository.DeleteBooking(bookingId);
  }

  public virtual async Task<BookingView?> GetBookingById(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    try
    {
      var booking = await _bookingRepository.GetBookingById(bookingId);

      var flight = await _flightRepository.GetFlightByFlightNumber(booking!.FlightNumber);

      var originAirport = await _airportRepository.GetAirportById(flight!.Origin);
      var destinationAirport = await _airportRepository.GetAirportById(flight.Destination);

      FlightView flightView = new(flight.FlightNumber,
                                 (originAirport!.City, originAirport.Iata),
                                 (destinationAirport!.City, destinationAirport.Iata));

      return new BookingView(bookingId, booking.Customer!.CustomerId, booking.Customer.Name, flightView);
    }
    catch (NullReferenceException)
    {
      return null;
    }
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

