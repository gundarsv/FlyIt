using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IGoogleCloudStorageService
    {
        public Task<Result<FileDTO>> UploadFileAsync(IFormFile imageFile);

        public Task<Result<string>> DeleteFileAsync(string fileName);
    }
}