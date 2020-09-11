using FlyIt.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace FlyIt.Api.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params Roles[] allowedRoles)
        {
            var allowedRolesAsStrings = allowedRoles.Select(x => Enum.GetName(typeof(Roles), x));
            Roles = string.Join(",", allowedRolesAsStrings);
        }
    }
}
