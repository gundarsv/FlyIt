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



        //how to replace the aviationstackFlightService?
        public async Task<Result<AirportDTO>> AddAirport(ClaimsPrincipal claims, AirportSearchDTO airportDTO)
        {
            try
            {
                var airport = await aviationstackFlightService.GetFlight( );

                if (flight is null)
                {
                    return new InvalidResult<FlightDTO>("Flight was not found");
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

        public async Task<Result<string>> DeleteAirport(ClaimsPrincipal claims, int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                var userAirport = repository.GetUserAirportById(user, id);

                if (userAirport == null)
                {
                    return new InvalidResult<string>("Airport is not assigned to user");
                }

                var removedUserAirport = repository.RemoveUserAirport(userAirport);

                if (removedUserAirport == null)
                {
                    return new InvalidResult<string>("Airport was not removed");
                }

                return new SuccessResult<string>("User Airport is removed");
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<string>(ex.Message);
            }
        }


        //how to replace the aviationstackFlightService?
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

        public async Task<Result<List<AirportDTO>>> GetUserAirports(ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                var userAirports = repository.GetUserAirports(user);

                if (userAirports.Count < 1)
                {
                    return new InvalidResult<List<AirportDTO>>("User has no airports");
                }

                var result = mapper.Map<List<Entity.UserAirport>, List<AirportDTO>>(userAirports);

                return new SuccessResult<List<AirportDTO>>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<List<AirportDTO>>(ex.Message);
            }
        }

        //how to replace the aviationstackFlightService?

        public async Task<Result<AirportSearchDTO>> SearchAirport(string iata)
        {
            try
            {
                var airport = await aviationstackFlightService.GetFlight(flightNo);

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