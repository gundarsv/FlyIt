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

        public FlightService(IAviationstackFlightService aviationstackFlightService, IMapper mapper, IFlightRepository repository, UserManager<User> userManager)
        {
            this.aviationstackFlightService = aviationstackFlightService;
            this.mapper = mapper;
            this.repository = repository;
            this.userManager = userManager;
        }

        public async Task<Result<FlightDTO>> AddFlight(ClaimsPrincipal claims, FlightSearchDTO flightDTO)
        {
            try
            {
                var flight = await aviationstackFlightService.GetFlight(flightDTO.FlightNo);

                if (flight is null)
                {
                    return new NotFoundResult<FlightDTO>("Flight was not found");
                }

                var mappingResult = mapper.Map<FlightsResponse, Entity.Flight>(flight);

                var flightInDatabase = repository.GetFlightByDateAndFlightNumber(mappingResult.Date, mappingResult.FlightNo);

                var user = await userManager.GetUserAsync(claims);

                if (flightInDatabase is null)
                {
                    var newFlight = repository.AddFlight(mappingResult);

                    var addedNewFlight = repository.AddUserFlight(user, newFlight);

                    var addedNewFlightMapping = mapper.Map<Entity.Flight, FlightDTO>(addedNewFlight.Flight);

                    return new SuccessResult<FlightDTO>(addedNewFlightMapping);
                }

                var userFlight = repository.GetUserFlight(user, flightInDatabase);

                if (userFlight is null)
                {
                    var addedFlight = repository.AddUserFlight(user, flightInDatabase);

                    var addedFlightMapping = mapper.Map<Entity.Flight, FlightDTO>(addedFlight.Flight);

                    return new SuccessResult<FlightDTO>(addedFlightMapping);
                }

                return new InvalidResult<FlightDTO>("Flight is already assigned to user");
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<FlightDTO>(ex.Message);
            }
        }

        public async Task<Result<string>> DeleteFlight(ClaimsPrincipal claims, int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                var userFlight = repository.GetUserFlightById(user, id);

                if (userFlight == null)
                {
                    return new InvalidResult<string>("Flight is not assigned to user");
                }

                var removedUserFlight = repository.RemoveUserFlight(userFlight);

                if (removedUserFlight == null)
                {
                    return new InvalidResult<string>("Flight was not removed");
                }

                return new SuccessResult<string>("User Flight is removed");
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<string>(ex.Message);
            }
        }

        public async Task<Result<FlightDTO>> GetFlight(ClaimsPrincipal claims, int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                var userFlight = repository.GetUserFlightById(user, id);

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

                var mappingResult = mapper.Map<FlightsResponse, Entity.Flight>(flightData);

                mappingResult.Id = userFlight.Flight.Id;
                mappingResult.UserFlights = userFlight.Flight.UserFlights;

                CompareLogic compareLogic = new CompareLogic();

                ComparisonResult cr = compareLogic.Compare(mappingResult, userFlight.Flight);

                if (cr.AreEqual)
                {
                    return new SuccessResult<FlightDTO>(result);
                }

                var updatedFlight = repository.UpdateFlight(userFlight.FlightId, mappingResult);

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

                var userFlights = repository.GetUserFlights(user);

                if (userFlights.Count < 1)
                {
                    return new InvalidResult<List<FlightDTO>>("User has no flights");
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

                if (!IsFlightDateValid(flight.Data[0].FlightDate.Date))
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