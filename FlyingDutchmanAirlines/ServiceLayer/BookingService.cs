using System.Reflection;
using System.Runtime.CompilerServices;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
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

  public virtual async Task<(bool, Exception?)> CreateBooking(string customerName, int flightNumber)
  {
    if (string.IsNullOrWhiteSpace(customerName) || int.IsNegative(flightNumber))
    {
      return (false, new ArgumentException("Invalid flight number or empty username provided"));
    }

    try
    {
      if (!await FlightExistsInDatabase(flightNumber))
      {
        return (false, new CouldNotAddBookingToDatabaseException());
      }

      Customer customer =
        await GetCustomerFromDatabase(customerName)
        ?? await AddCustomerToDatabase(customerName);

      await _bookingRepository.CreateBooking(customer.CustomerId, flightNumber);
      return (true, null);
    }
    catch (Exception ex)
    {
      return (false, ex);
    }
  }

  public virtual async Task DeleteBooking(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    await _bookingRepository.DeleteBooking(bookingId);
  }

  public virtual async Task<BookingView> GetBookingById(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    Booking booking = await _bookingRepository.GetBookingById(bookingId);

    Flight flight = await _flightRepository.GetFlightByFlightNumber(booking.FlightNumber);
    Airport originAirport = await _airportRepository.GetAirportByID(flight.Origin);
    Airport destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);

    FlightView flightView = new(flight.FlightNumber,
                                (originAirport.City, originAirport.Iata),
                                (destinationAirport.City, destinationAirport.Iata));

    return new BookingView(bookingId, booking.Customer!.CustomerId, booking.Customer.Name, flightView);
  }

  public virtual async IAsyncEnumerable<BookingView> GetBookingsByCustomerName(string customerName)
  {
    Customer customer;

    customer = await GetCustomerFromDatabase(customerName) ?? throw new BookingNotFoundException($"Customer {customerName} not found");
    if (!customer.Bookings.Any()) throw new BookingNotFoundException($"No bookings for {customerName} in database");

    foreach (Booking booking in customer.Bookings)
    {
      yield return await GetBookingById(booking.BookingId);
    }
  }

  private async Task<bool> FlightExistsInDatabase(int flightNumber)
  {
    try
    {
      return await _flightRepository.GetFlightByFlightNumber(flightNumber) != null;
    }
    catch (FlightNotFoundException)
    {
      return false;
    }
  }

  private async Task<Customer?> GetCustomerFromDatabase(string name)
  {
    try
    {
      return await _customerRepository.GetCustomerByName(name);
    }
    catch (CustomerNotFoundException)
    {
      return null;
    }
    catch (Exception)
    {
      throw;
    }
  }

  private async Task<Customer> AddCustomerToDatabase(string name)
  {
    await _customerRepository.CreateCustomer(name);
    return await _customerRepository.GetCustomerByName(name);
  }
}

