using System;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.InfrastuctureLayer;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines_Tests.Stubs;

public class FlyingDutchmanAirlinesContext_Stub : FlyingDutchmanAirlinesContext
{
  public FlyingDutchmanAirlinesContext_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options)
    : base (options)
  {
    base.Database.EnsureDeleted();
  }

  public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    IEnumerable<EntityEntry> pendingChanges =
      ChangeTracker.Entries()
                   .Where(e => e.State == EntityState.Added);

    IEnumerable<Booking> bookings =
      pendingChanges
      .Select(e => e.Entity)
      .OfType<Booking>();

    if (bookings.Any(b => b.CustomerId != 1))
    {
      throw new DbUpdateException("Database error!");
    }

    IEnumerable<Customer> customers =
      pendingChanges
      .Select(c => c.Entity)
      .OfType<Customer>();

    if (customers.Any(c => c.Name == "Db Fail"))
    {
      throw new DbUpdateException("Database error!");
    }

    return await base.SaveChangesAsync(cancellationToken);
  }
}

