using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.AviationstackResponses;
using FlyIt.Domain.ServiceResult;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Entity = FlyIt.DataAccess.Entities;

namespace FlyIt.Domain.Services
{
    public class FlightService : IFlightService
    {
        private readonly IAviationstackFlightService aviationstackFlightService;
        private readonly IMapper mapper;
        private readonly IFlightRepository repository;
        private readonly UserManager<User> userManager;
        private readonly ICompareLogic compareLogic;

        public FlightService(IAviationstackFlightService aviationstackFlightService, IMapper mapper, IFlightRepository repository, UserManager<User> userManager, ICompareLogic compareLogic)
        {
            this.aviationstackFlightService = aviationstackFlightService;
            this.mapper = mapper;
            this.repository = repository;
            this.userManager = userManager;
            this.compareLogic = compareLogic;
        }

        public async Task<Result<FlightDTO>> AddFlight(ClaimsPrincipal claims, FlightSearchDTO flightDTO)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<FlightDTO>("User was not found");
                }

                var flight = await aviationstackFlightService.GetFlight(flightDTO.FlightNo);

                if (flight is null)
                {
                    return new NotFoundResult<FlightDTO>("Flight was not found");
                }

                var flightInDatabase = await repository.GetFlightByDateAndFlightNumberAsync(flight.Data.FirstOrDefault().FlightDate, flight.Data.FirstOrDefault().Flight.Iata);

                if (flightInDatabase is null)
                {
                    var mappingResult = mapper.Map<FlightsResponse, Entity.Flight>(flight);

                    var newFlight = await repository.AddFlightAsync(mappingResult);

                    if (newFlight is null)
                    {
                        return new InvalidResult<FlightDTO>("Flight not added");
                    }

                    var addedNewFlight = await repository.AddUserFlightAsync(user, newFlight);

                    if (addedNewFlight is null)
                    {
                        return new InvalidResult<FlightDTO>("Flight not added to user");
                    }

                    var addedNewFlightMapping = mapper.Map<Entity.Flight, FlightDTO>(addedNewFlight.Flight);

                    return new SuccessResult<FlightDTO>(addedNewFlightMapping);
                }

                var userFlight = await repository.GetUserFlightByIdAsync(user.Id, flightInDatabase.Id);

                if (userFlight != null)
                {
                    return new InvalidResult<FlightDTO>("Flight is already assigned to user");
                }

                var addedFlight = await repository.AddUserFlightAsync(user, flightInDatabase);

                if (addedFlight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight is already assigned to user");
                }

                var addedFlightMapping = mapper.Map<Entity.Flight, FlightDTO>(addedFlight.Flight);

                return new SuccessResult<FlightDTO>(addedFlightMapping);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<FlightDTO>(ex.Message);
            }
        }

        public async Task<Result<FlightDTO>> DeleteFlight(ClaimsPrincipal claims, int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<FlightDTO>("User was not found");
                }

                var flight = await repository.GetFlightByIdAsync(id);

                if (flight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight not found");
                }

                var userFlight = await repository.GetUserFlightByIdAsync(user.Id, flight.Id);

                if (userFlight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight is not assigned to user");
                }

                var removedUserFlight = await repository.RemoveUserFlightAsync(userFlight);

                if (removedUserFlight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight was not removed");
                }

                var result = mapper.Map<Entity.Flight, FlightDTO>(removedUserFlight.Flight);

                return new SuccessResult<FlightDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<FlightDTO>(ex.Message);
            }
        }

        public async Task<Result<FlightDTO>> GetFlight(ClaimsPrincipal claims, int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<FlightDTO>("User was not found");
                }

                var flight = await repository.GetFlightByIdAsync(id);

                if (flight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight not found");
                }

                var userFlight = await repository.GetUserFlightByIdAsync(user.Id, flight.Id);

                if (userFlight is null)
                {
                    return new InvalidResult<FlightDTO>("User has no flight with ID: " + id);
                }

                bool canBeUpdated = IsFlightDateValid(userFlight.Flight.Date.Date);

                var result = mapper.Map<Entity.Flight, FlightDTO>(userFlight.Flight);

                if (!canBeUpdated)
                {
                    return new SuccessResult<FlightDTO>(result);
                }

                var flightData = await aviationstackFlightService.GetFlight(userFlight.Flight.FlightNo);

                if (flightData is null)
                {
                    return new SuccessResult<FlightDTO>(result);
                }

                var mappingResult = mapper.Map<FlightsResponse, Entity.Flight>(flightData);

                if (mappingResult.Date != result.Date)
                {
                    return new SuccessResult<FlightDTO>(result);
                }

                mappingResult.Id = userFlight.Flight.Id;
                mappingResult.UserFlights = userFlight.Flight.UserFlights;

                ComparisonResult cr = compareLogic.Compare(mappingResult, userFlight.Flight);

                if (cr.AreEqual)
                {
                    return new SuccessResult<FlightDTO>(result);
                }

                var updatedFlight = await repository.UpdateFlightAsync(mappingResult);

                if (updatedFlight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight was not updated");
                }

                result = mapper.Map<Entity.Flight, FlightDTO>(updatedFlight);

                return new SuccessResult<FlightDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<FlightDTO>(ex.Message);
            }
        }

        public async Task<Result<List<FlightDTO>>> GetUserFlights(ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<List<FlightDTO>>("User not found");
                }

                var userFlights = await repository.GetUserFlightsAsync(user);

                if (userFlights.Count < 1)
                {
                    return new NotFoundResult<List<FlightDTO>>("User has no flights");
                }

                var result = mapper.Map<List<Entity.UserFlight>, List<FlightDTO>>(userFlights);

                return new SuccessResult<List<FlightDTO>>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<List<FlightDTO>>(ex.Message);
            }
        }

        public async Task<Result<FlightSearchDTO>> SearchFlight(string flightNo)
        {
            try
            {
                var flight = await aviationstackFlightService.GetFlight(flightNo);

                if (flight is null)
                {
                    return new InvalidResult<FlightSearchDTO>("Flight was not found");
                }

                if (!IsFlightDateValid(flight.Data.FirstOrDefault().FlightDate.Date))
                {
                    return new InvalidResult<FlightSearchDTO>("Flight was not found");
                }

                var result = mapper.Map<FlightsResponse, FlightSearchDTO>(flight);

                return new SuccessResult<FlightSearchDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<FlightSearchDTO>(ex.Message);
            }
        }

        private bool IsFlightDateValid(DateTime date) => date.Date >= DateTime.Now.Date;
    }
}