﻿using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.InfrastuctureLayer.Models;


namespace FlyingDutchmanAirlines.InfrastuctureLayer;

public class BookingRepository : IBookingRepository
{
  private readonly FlyingDutchmanAirlinesContext _context = null!;

  public BookingRepository(FlyingDutchmanAirlinesContext injectedContext)
  {
    _context = injectedContext;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public BookingRepository()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new InvalidOperationException("This constructor should only be used for testing");
    }
  }

  public async Task<bool> CreateBooking(int customerId, int flightNumber)
  {
    var newBooking = Booking.Create(customerId, flightNumber);

    _context.Bookings.Add(newBooking);
    var result = await _context.SaveChangesAsync();

    return result > 0;
  }

  public async Task<bool> AddBooking(Booking booking)
  {
    if (booking is null)
    {
      return false;
    }

    try
    {
      _context.Bookings.Add(booking);
      var result = await _context.SaveChangesAsync();

      return result > 0;
    }
    catch (DbUpdateException ex)
    {
      Console.WriteLine($"Error caught {ex.Message} - SaveChangesAsync failed");
      return false;
    }
  }

  public async Task<Booking?> GetBookingById(int bookingId)
  {
    if (!await _context.Bookings.AnyAsync(b => b.BookingId == bookingId))
    {
      return null;
    }

    return await _context.Bookings.Include(b => b.Customer)
                                  .Include(b => b.FlightNumberNavigation.DestinationNavigation)
                                  .Include(b => b.FlightNumberNavigation.OriginNavigation)
                                  .FirstOrDefaultAsync(b => b.BookingId == bookingId);
  }

  public async Task<bool> DeleteBooking(int bookingId)
  {
    if (bookingId < 0)
    {
      throw new ArgumentException("Invalid booking id - Negative id");
    }

    var booking = await _context.Bookings.FindAsync(bookingId);

    if (booking is null)
    {
      return false;
    }

    _context.Bookings.Remove(booking);
    await _context.SaveChangesAsync();

    return true;
  }
}

