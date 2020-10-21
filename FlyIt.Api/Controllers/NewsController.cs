using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Api.Models;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService)
        {
            this.newsService = newsService;
        }

        [AuthorizeRoles(Roles.AirportsAdministrator)]
        [HttpPost("{airportId}")]
        public async Task<IActionResult> AddNews(News news, int airportId)
        {
            var result = await newsService.AddNews(news.Title, news.Imageurl, news.ImageName, news.Body, airportId, User);

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.AirportsAdministrator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var result = await newsService.DeleteNews(id, User);

            return this.FromResult(result);
        }

        [HttpGet("airport/{airportId}")]
        public async Task<IActionResult> GetNewsByAirportId(int airportId)
        {
            var result = await newsService.GetNewsByAirportId(airportId);

            return this.FromResult(result);
        }

        [HttpGet("airport/{iata}")]
        public async Task<IActionResult> GetNewsByAirportIata(string iata)
        {
            var result = await newsService.GetNewsByAirportIata(iata);

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.AirportsAdministrator)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody, Required] News news)
        {
            var result = await newsService.UpdateNews(id, news.Title, news.Imageurl, news.ImageName, news.Body, User);

            return this.FromResult(result);
        }
    }
}