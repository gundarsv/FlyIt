using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IGoogleCloudStorageService
    {
        public Task<Result<ImageDTO>> UploadImageAsync(IFormFile imageFile);

        public Task<Result<string>> DeleteImageAsync(string fileName);
    }
}