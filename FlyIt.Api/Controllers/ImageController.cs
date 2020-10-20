using System.Threading.Tasks;
using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyIt.Api.Controllers
{
    [AuthorizeRoles(Roles.AirportsAdministrator)]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IGoogleCloudStorageService googleCloudStorageService;

        public ImageController(IGoogleCloudStorageService googleCloudStorageService)
        {
            this.googleCloudStorageService = googleCloudStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image, string fileName)
        {
            var result = await googleCloudStorageService.UploadImageAsync(image, fileName);

            return this.FromResult(result);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            var result = await googleCloudStorageService.DeleteImageAsync(fileName);

            return this.FromResult(result);
        }
    }
}