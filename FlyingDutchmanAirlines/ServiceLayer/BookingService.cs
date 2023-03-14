using System;
using System.Runtime.ExceptionServices;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class BookingService
{
  private readonly BookingRepository _bookingRepository;
  private readonly CustomerRepository _customerRepository;
  private readonly FlightRepository _flightRepository;

  public BookingService(CustomerRepository customerRepository, BookingRepository bookingRepository, FlightRepository flightRepository)
  {
    _customerRepository = customerRepository;
    _bookingRepository = bookingRepository;
    _flightRepository = flightRepository;
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

