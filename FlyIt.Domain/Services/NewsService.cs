using AutoMapper;
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
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class NewsService : INewsService
    {
        private readonly UserManager<User> userManager;
        private readonly IAirportRepository airportRepository;
        private readonly INewsRepository newsRepository;
        private readonly IMapper mapper;
        private readonly IGoogleCloudStorageService googleCloudStorageService;

        public NewsService(UserManager<User> userManager, IAirportRepository airportRepository, INewsRepository newsRepository, IMapper mapper, IGoogleCloudStorageService googleCloudStorageService)
        {
            this.userManager = userManager;
            this.airportRepository = airportRepository;
            this.newsRepository = newsRepository;
            this.mapper = mapper;
            this.googleCloudStorageService = googleCloudStorageService;
        }

        public async Task<Result<NewsDTO>> AddNews(string title, string imageurl, string imageName, string body, int airportId, ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<NewsDTO>("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                if (userRoles is null || !userRoles.Contains(Roles.AirportsAdministrator.ToString()))
                {
                    return new InvalidResult<NewsDTO>($"User is not in role: {Roles.AirportsAdministrator}");
                }

                var airport = await airportRepository.GetAirportByIdAsync(airportId);

                if (airport is null)
                {
                    return new NotFoundResult<NewsDTO>("Airport not found");
                }

                var userAirport = await airportRepository.GetUserAirportByIdAsync(user.Id, airport.Id);

                if (userAirport is null)
                {
                    return new InvalidResult<NewsDTO>("User not assigned to this airport");
                }

                var addedNews = await newsRepository.AddNewsAsync(new News()
                {
                    Title = title,
                    Imageurl = imageurl,
                    ImageName = imageName,
                    Body = body,
                    AirportId = airportId,
                    Airport = airport
                });

                if (addedNews is null)
                {
                    return new InvalidResult<NewsDTO>("News was not created");
                }

                var result = mapper.Map<News, NewsDTO>(addedNews);

                return new SuccessResult<NewsDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<NewsDTO>(ex.Message);
            }
        }

        public async Task<Result<List<NewsDTO>>> GetNews(int airportId)
        {
            try
            {
                var airport = await airportRepository.GetAirportByIdAsync(airportId);

                if (airport is null)
                {
                    return new NotFoundResult<List<NewsDTO>>("Airport not found");
                }

                var news = await newsRepository.GetNewsByAirportIdAsync(airport.Id);

                if (news.Count < 1)
                {
                    return new NotFoundResult<List<NewsDTO>>("Airport does not have news");
                }

                var result = mapper.Map<List<News>, List<NewsDTO>>(news);

                return new SuccessResult<List<NewsDTO>>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<List<NewsDTO>>(ex.Message);
            }
        }

        public async Task<Result<NewsDTO>> DeleteNews(int id, ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<NewsDTO>("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                if (userRoles is null || !userRoles.Contains(Roles.AirportsAdministrator.ToString()))
                {
                    return new InvalidResult<NewsDTO>($"User is not in role: {Roles.AirportsAdministrator}");
                }

                var news = await newsRepository.GetNewsByIdAsync(id);

                if (news is null)
                {
                    return new NotFoundResult<NewsDTO>("News item not found");
                }

                var deletedNews = await newsRepository.RemoveNewsAsync(news);

                if (deletedNews is null)
                {
                    return new InvalidResult<NewsDTO>($"News {news} can not be deleted");
                }

                var oldImageDelete = await googleCloudStorageService.DeleteImageAsync(deletedNews.ImageName);

                if (!oldImageDelete.ResultType.Equals(ResultType.Ok))
                {
                    return new InvalidResult<NewsDTO>("Image was not deleted");
                }

                var result = mapper.Map<News, NewsDTO>(deletedNews);

                return new SuccessResult<NewsDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<NewsDTO>(ex.Message);
            }
        }

        public async Task<Result<NewsDTO>> UpdateNews(int id, string Title, string Imageurl, string imageName, string Body, ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<NewsDTO>("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                if (userRoles is null || !userRoles.Contains(Roles.AirportsAdministrator.ToString()))
                {
                    return new InvalidResult<NewsDTO>($"User is not in role: {Roles.AirportsAdministrator}");
                }

                var news = await newsRepository.GetNewsByIdAsync(id);

                if (news is null)
                {
                    return new NotFoundResult<NewsDTO>("News item not found");
                }

                var airport = await airportRepository.GetAirportByIdAsync(news.AirportId);

                if (airport is null)
                {
                    return new NotFoundResult<NewsDTO>("Airport not found");
                }

                var userAirport = await airportRepository.GetUserAirportByIdAsync(user.Id, airport.Id);

                if (userAirport is null)
                {
                    return new InvalidResult<NewsDTO>("User not assigned to this airport");
                }

                var updatedNews = await newsRepository.UpdateNewsAsync(new News() { Id = news.Id, Title = Title, Imageurl = Imageurl, ImageName = imageName, Body = Body, AirportId = news.AirportId, Airport = news.Airport });

                if (updatedNews is null)
                {
                    return new InvalidResult<NewsDTO>("News was not updated");
                }

                var result = mapper.Map<News, NewsDTO>(updatedNews);

                if (news.ImageName == updatedNews.ImageName)
                {
                    return new SuccessResult<NewsDTO>(result);
                }

                var oldImageDelete = await googleCloudStorageService.DeleteImageAsync(news.ImageName);

                if (!oldImageDelete.ResultType.Equals(ResultType.Ok))
                {
                    return new InvalidResult<NewsDTO>("Old Image was not deleted");
                }

                return new SuccessResult<NewsDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<NewsDTO>(ex.Message);
            }
        }
    }
}