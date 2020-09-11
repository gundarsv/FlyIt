﻿using System;

namespace FlyIt.Api.Models
{
    [Flags]
    public enum Roles
    {
        User = 1,
        AirportsAdministrator = 2,
        SystemAdministrator = 3,
    }
}
