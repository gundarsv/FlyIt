using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly ICheckWXAPIMetarService checkWXAPIMetarService;
        private readonly ICheckWXAPIStationService checkWXAPIStationService;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public WeatherService(ICheckWXAPIMetarService checkWXAPIMetarService, UserManager<User> userManager, IMapper mapper, ICheckWXAPIStationService checkWXAPIStationService)
        {
            this.checkWXAPIMetarService = checkWXAPIMetarService;
            this.checkWXAPIStationService = checkWXAPIStationService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<Result<WeatherDTO>> GetWeatherByIcao(string icao, ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<WeatherDTO>("User not found");
                }

                var metarResult = await checkWXAPIMetarService.GetMetarByICAO(icao);

                if (metarResult is null)
                {
                    return new NotFoundResult<WeatherDTO>("Metar not found");
                }

                var result = mapper.Map<MetarResponse, WeatherDTO>(metarResult);

                var stationResult = await checkWXAPIStationService.GetStationByICAO(icao);

                if (stationResult != null)
                {
                    result.City = stationResult.Data[0].City;
                }

                return new SuccessResult<WeatherDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<WeatherDTO>(ex.Message);
            }
        }
    }
}