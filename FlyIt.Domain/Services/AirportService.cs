using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Entity = FlyIt.DataAccess.Entities;

namespace FlyIt.Domain.Services
{
    public class AirportService : IAirportService
    {
        private readonly IMapper mapper;
        private readonly IAirportRepository repository;
        private readonly UserManager<User> userManager;

        public AirportService(IMapper mapper, IAirportRepository repository, UserManager<User> userManager)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.userManager = userManager;
        }

        public async Task<Result<List<AirportDTO>>> GetUserAirports(ClaimsPrincipal claims)
        {
            try
            {

                var user = await userManager.GetUserAsync(claims);

                var airports = repository.GetUserAirports(user);


                if (airports.Count < 1)
                {
                    return new InvalidResult<List<AirportDTO>>("User has no airports");
                }

                var result = mapper.Map<List<UserAirport>, List<AirportDTO>>(airports);

                return new SuccessResult<List<AirportDTO>>(result);
            }

            catch (Exception exc)
            {
                return new UnexpectedResult<List<AirportDTO>>(exc.Message);
            }
        }
    }
}