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
    public class FileController : ControllerBase
    {
        private readonly IGoogleCloudStorageService googleCloudStorageService;

        public FileController(IGoogleCloudStorageService googleCloudStorageService)
        {
            this.googleCloudStorageService = googleCloudStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var result = await googleCloudStorageService.UploadFileAsync(file);

            return this.FromResult(result);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var result = await googleCloudStorageService.DeleteFileAsync(fileName);

            return this.FromResult(result);
        }
    }
}