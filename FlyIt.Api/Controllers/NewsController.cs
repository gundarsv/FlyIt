using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Api.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyIt.Api.Controllers
{
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
            var result = await newsService.AddNews(news.Title, news.Imageurl, news.Body, airportId, User);

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.AirportsAdministrator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var result = await newsService.DeleteNews(id, User);

            return this.FromResult(result);
        }
    }
}