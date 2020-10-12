using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IGoogleCloudStorageService
    {
        public Task<string> UploadImageAsync(IFormFile imageFile, string fileName);

        public Task<string> DeleteImageAsync(string fileName);
    }
}
