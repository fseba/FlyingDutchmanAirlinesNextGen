using System;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

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
      throw new Exception("Database error!");
    }

    return await base.SaveChangesAsync(cancellationToken);
  }
}

