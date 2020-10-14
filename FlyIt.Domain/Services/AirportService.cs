﻿using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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

        public async Task<Result<AirportDTO>> AddAirport(string Iata, string Name, ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<AirportDTO>("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                if (userRoles is null || !userRoles.Contains(Roles.AirportsAdministrator.ToString()))
                {
                    return new InvalidResult<AirportDTO>($"User is not in role: {Roles.AirportsAdministrator}");
                }

                var airportInDatabase = await repository.GetAirportByIataAsync(Iata);

                if (airportInDatabase != null)
                {
                    return new InvalidResult<AirportDTO>("Airport already exists");
                }

                var addedAirport = await repository.AddAirportAsync(new Airport() { Iata = Iata, Name = Name });

                if (addedAirport is null)
                {
                    return new InvalidResult<AirportDTO>("Airport not created");
                }

                var addedUserAirport = await repository.AddUserAirportAsync(user, addedAirport);

                if (addedUserAirport is null)
                {
                    return new InvalidResult<AirportDTO>("Airport was not added to user");
                }

                var result = mapper.Map<UserAirport, AirportDTO>(addedUserAirport);

                return new CreatedResult<AirportDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AirportDTO>(ex.Message);
            }
        }

        public async Task<Result<AirportDTO>> AddAirportToUser(int airportId, int userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user is null)
                {
                    return new NotFoundResult<AirportDTO>("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                if (userRoles is null || !userRoles.Contains(Roles.AirportsAdministrator.ToString()))
                {
                    return new InvalidResult<AirportDTO>($"User is not in role: {Roles.AirportsAdministrator}");
                }

                var airport = await repository.GetAirportByIdAsync(airportId);

                if (airport is null)
                {
                    return new NotFoundResult<AirportDTO>("Airport not found");
                }

                var userAirport = await repository.AddUserAirportAsync(user, airport);

                if (userAirport is null)
                {
                    return new InvalidResult<AirportDTO>("Airport cannot be added");
                }

                var result = mapper.Map<UserAirport, AirportDTO>(userAirport);

                return new CreatedResult<AirportDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AirportDTO>(ex.Message);
            }
        }

        public async Task<Result<AirportDTO>> DeleteAirport(int id, ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<AirportDTO>("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                if (userRoles is null || !userRoles.Contains(Roles.AirportsAdministrator.ToString()))
                {
                    return new InvalidResult<AirportDTO>($"User is not in role: {Roles.AirportsAdministrator}");
                }

                var airport = await repository.GetAirportByIdAsync(id);

                if (airport is null)
                {
                    return new NotFoundResult<AirportDTO>("Airport not found");
                }

                var userAirport = await repository.GetUserAirportByIdAsync(user.Id, airport.Id);

                if (userAirport is null)
                {
                    return new InvalidResult<AirportDTO>($"User does not have airport with Id: {airport.Id}");
                }

                var deletedAirport = await repository.RemoveAirportAsync(userAirport.Airport);

                if (deletedAirport is null)
                {
                    return new InvalidResult<AirportDTO>($"Airport {userAirport.AirportId} can not be deleted by user {userAirport.UserId}");
                }

                var result = mapper.Map<Airport, AirportDTO>(deletedAirport);

                return new SuccessResult<AirportDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AirportDTO>(ex.Message);
            }
        }

        public async Task<Result<List<AirportDTO>>> GetAllAirports()
        {
            try
            {
                var airports = await repository.GetAirportsAsync();

                if (airports.Count < 1)
                {
                    return new NotFoundResult<List<AirportDTO>>("Airports not found");
                }

                var result = mapper.Map<List<Airport>, List<AirportDTO>>(airports);

                return new SuccessResult<List<AirportDTO>>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<List<AirportDTO>>(ex.Message);
            }
        }

        public async Task<Result<List<AirportDTO>>> GetUserAirports(ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                var airports = await repository.GetUserAirportsAsync(user);

                if (airports.Count < 1)
                {
                    return new NotFoundResult<List<AirportDTO>>("User has no airports");
                }

                var result = mapper.Map<List<UserAirport>, List<AirportDTO>>(airports);

                return new SuccessResult<List<AirportDTO>>(result);
            }
            catch (Exception exc)
            {
                return new UnexpectedResult<List<AirportDTO>>(exc.Message);
            }
        }

        public async Task<Result<AirportDTO>> RemoveAirportFromUser(int airportId, int userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());

                if (user is null)
                {
                    return new NotFoundResult<AirportDTO>("User not found");
                }

                var airport = await repository.GetAirportByIdAsync(airportId);

                if (airport is null)
                {
                    return new NotFoundResult<AirportDTO>("Airport not found");
                }

                var userAirport = await repository.GetUserAirportByIdAsync(userId, airportId);

                if (userAirport is null)
                {
                    return new NotFoundResult<AirportDTO>("User does not have this airport");
                }

                var deletedUserAirport = await repository.RemoveUserAirportAsync(userAirport);

                if (deletedUserAirport is null)
                {
                    return new InvalidResult<AirportDTO>("Airport not deleted from user");
                }

                var result = mapper.Map<UserAirport, AirportDTO>(deletedUserAirport);

                return new SuccessResult<AirportDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AirportDTO>(ex.Message);
            }
        }
    }
}