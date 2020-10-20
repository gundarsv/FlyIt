using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IGoogleCloudStorageService
    {
        public Task<Result<string>> UploadImageAsync(IFormFile imageFile, string fileName);

        public Task<Result<string>> DeleteImageAsync(string fileName);
    }
}