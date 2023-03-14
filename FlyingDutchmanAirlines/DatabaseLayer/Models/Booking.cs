﻿using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public sealed class Booking
{
    public int BookingId { get; set; }

    public int FlightNumber { get; set; }

    public int? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Flight FlightNumberNavigation { get; set; } = null!;
}
