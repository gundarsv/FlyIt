using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.AviationstackResponses;
using FlyIt.Domain.ServiceResult;
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

        public async Task<Result<FlightDTO>> AddFlight(ClaimsPrincipal claims, NoIdFlightDTO flightDTO)
        {
            try
            {
                var flight = await aviationstackFlightService.GetFlight(flightDTO.FlightNo);

                if (!IsFlightValid(flight))
                {
                    if (flight.Errors.Count > 0)
                    {
                        return new InvalidResult<FlightDTO>(flight.Errors[0]);
                    }

                    return new InvalidResult<FlightDTO>("Flight was not found");
                }

                var mappingResult = mapper.Map<FlightsResponse, Entity.Flight>(flight.Data);

                var flightInDatabase = repository.GetFlightByDateAndFlightNumber(mappingResult.Date, mappingResult.FlightNo);

                var user = await userManager.GetUserAsync(claims);

                if (flightInDatabase == null)
                {
                    var newFlight = repository.AddFlight(mappingResult);

                    var addedNewFlight = repository.AddUserFlight(user, newFlight);

                    var addedNewFlightMapping = mapper.Map<Entity.UserFlight, FlightDTO>(addedNewFlight);

                    return new SuccessResult<FlightDTO>(addedNewFlightMapping);
                }

                var userFlights = repository.GetUserFlights(user);

                if (UserHasFlight(flightInDatabase, userFlights))
                {
                    return new InvalidResult<FlightDTO>("Flight is already assigned to user");
                }

                var addedFlight = repository.AddUserFlight(user, flightInDatabase);

                var addedFlightMapping = mapper.Map<Entity.UserFlight, FlightDTO>(addedFlight);

                return new SuccessResult<FlightDTO>(addedFlightMapping);
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

        public async Task<Result<List<FlightDTO>>> GetUserFlights(ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                var userFlights = repository.GetUserFlights(user);

                if (userFlights.Count < 1 )
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

        public async Task<Result<NoIdFlightDTO>> GetFlight(string flightNo)
        {
            try
            {
                var flight = await aviationstackFlightService.GetFlight(flightNo);

                if (IsFlightValid(flight))
                {
                    var result = mapper.Map<FlightsResponse, NoIdFlightDTO>(flight.Data);

                    return new SuccessResult<NoIdFlightDTO>(result);
                }

                if (flight.Errors.Count > 0)
                {
                    return new InvalidResult<NoIdFlightDTO>(flight.Errors[0]);
                }

                return new InvalidResult<NoIdFlightDTO>("Flight was not found");
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<NoIdFlightDTO>(ex.Message);
            }
        }

        private bool IsFlightValid(Result<FlightsResponse> response)
        {
            if (!response.ResultType.Equals(ResultType.Ok))
            {
                return false;
            }

            if (response.Data.Data == null)
            {
                return false;
            }

            if (response.Data.Data.Count < 1)
            {
                return false;
            }

            var flightDate = response.Data.Data[0].FlightDate.Date;

            return flightDate == DateTime.Now.Date;
        }

        private bool UserHasFlight(Entity.Flight flight, List<UserFlight> userFlights)
        {
            return userFlights.Exists(uf => uf.FlightId == flight.Id);
        }
    }
}
