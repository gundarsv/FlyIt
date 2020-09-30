﻿using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities
{
    public class Flight
    {
        public int Id { get; set; }

        public string FlightNo { get; set; }

        public DateTimeOffset Date { get; set; }

        public List<UserFlight> UserFlights { get; set; }
    }
}
