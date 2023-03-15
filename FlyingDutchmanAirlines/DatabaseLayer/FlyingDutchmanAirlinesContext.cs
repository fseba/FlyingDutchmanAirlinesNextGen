using FlyingDutchmanAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.DatabaseLayer;

public class FlyingDutchmanAirlinesContext : DbContext
{
    public FlyingDutchmanAirlinesContext()
    {
    }

    public FlyingDutchmanAirlinesContext(DbContextOptions<FlyingDutchmanAirlinesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      string connectionString =
      Environment.GetEnvironmentVariable("FlyingDutchmanAirlines_Database_Connection_String")
        ?? string.Empty;
      optionsBuilder.UseSqlServer(connectionString);
    }
  }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<Airport>(entity =>
      {
          entity.ToTable("Airport");

          entity.Property(e => e.AirportId)
              .ValueGeneratedNever()
              .HasColumnName("AirportID");
          entity.Property(e => e.City)
              .HasMaxLength(50)
              .IsUnicode(false);
          entity.Property(e => e.Iata)
              .HasMaxLength(3)
              .IsUnicode(false)
              .HasColumnName("IATA");
      });

      modelBuilder.Entity<Booking>(entity =>
      {
          entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD6A0AA415");

          entity.ToTable("Booking");

          entity.Property(e => e.BookingId).HasColumnName("BookingID");
          entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

          entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
              .HasForeignKey(d => d.CustomerId)
              .HasConstraintName("FK__Booking__Custome__71D1E811");

          entity.HasOne(d => d.FlightNumberNavigation).WithMany(p => p.Bookings)
              .HasForeignKey(d => d.FlightNumber)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__Booking__FlightN__4F7CD00D");
      });

      modelBuilder.Entity<Customer>(entity =>
      {
          entity.ToTable("Customer");

          entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
          entity.Property(e => e.Name)
              .HasMaxLength(50)
              .IsUnicode(false);
      });

      modelBuilder.Entity<Flight>(entity =>
      {
          entity.HasKey(e => e.FlightNumber);

          entity.ToTable("Flight");

          entity.Property(e => e.FlightNumber).ValueGeneratedNever();

          entity.HasOne(d => d.DestinationNavigation).WithMany(p => p.FlightDestinationNavigations)
              .HasForeignKey(d => d.Destination)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Flight_AirportDestination");

          entity.HasOne(d => d.OriginNavigation).WithMany(p => p.FlightOriginNavigations)
              .HasForeignKey(d => d.Origin)
              .OnDelete(DeleteBehavior.ClientSetNull);
      });
  }
}
