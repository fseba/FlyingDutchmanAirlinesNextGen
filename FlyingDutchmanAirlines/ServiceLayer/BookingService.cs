﻿using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class BookingService
{
  private readonly BookingRepository _bookingRepository;
  private readonly CustomerRepository _customerRepository;
  private readonly FlightRepository _flightRepository;
  private readonly AirportRepository _airportRepository;

  public BookingService(CustomerRepository customerRepository, BookingRepository bookingRepository, FlightRepository flightRepository, AirportRepository airportRepository)
  {
    _customerRepository = customerRepository;
    _bookingRepository = bookingRepository;
    _flightRepository = flightRepository;
    _airportRepository = airportRepository;
  }

  public async Task<(bool, Exception?)> CreateBooking(string customerName, int flightNumber)
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

  public async Task<(bool, Exception?)> DeleteBooking(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      return (false, new ArgumentException("Invalid booking id - Is negative"));
    }

    try
    {
      await _bookingRepository.DeleteBooking(bookingId);

      return (true, null);
    }
    catch (Exception ex)
    {
      return (false, ex);
    }
  }

  public async Task<BookingView> GetBookingById(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      throw new ArgumentException("Invalid booking id - Is negative");
    }

    try
    {
      Booking booking = await _bookingRepository.GetBookingById(bookingId);

      Flight flight = await _flightRepository.GetFlightByFlightNumber(booking.FlightNumber);
      Airport originAirport = await _airportRepository.GetAirportByID(flight.Origin);
      Airport destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);

      FlightView flightView = new(flight.FlightNumber,
                                 (originAirport.City, originAirport.Iata),
                                 (destinationAirport.City, destinationAirport.Iata));

      return new BookingView(bookingId, booking.Customer!.CustomerId, booking.Customer.Name, flightView);
    }
    catch (Exception)
    {
      throw;
    }
  }

  public async IAsyncEnumerable<BookingView> GetBookingsByCustomerName(string customerName)
  {
    Customer customer;

    try
    {
      customer = await GetCustomerFromDatabase(customerName) ?? throw new CustomerNotFoundException();
    }
    catch (Exception ex)
    {
      throw new BookingNotFoundException("Customer not found", ex.InnerException!);
    }

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

