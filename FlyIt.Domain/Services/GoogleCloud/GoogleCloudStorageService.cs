using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Settings;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class GoogleCloudStorageService : IGoogleCloudStorageService
    {
        private readonly GoogleCloudSettings googleCloudSettings;
        private readonly StorageClient storageClient;

        public GoogleCloudStorageService(IOptionsSnapshot<GoogleCloudSettings> googleCloudSettings, IStorageClientWrapper storageClientWrapper)
        {
            this.googleCloudSettings = googleCloudSettings.Value;
            this.storageClient = storageClientWrapper.GetStorageClient(googleCloudSettings.Value);
        }

        public async Task<Result<string>> UploadImageAsync(IFormFile imageFile, string fileName)
        {
            try
            {
                if (imageFile is null)
                {
                    return new InvalidResult<string>("Imagefile was not added");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);

                    var result = await storageClient.UploadObjectAsync(googleCloudSettings.GoogleCloudStorageBucket, fileName, imageFile.ContentType, memoryStream);

                    if (result is null)
                    {
                        return new InvalidResult<string>("Image was not uploaded");
                    }

                    return new SuccessResult<string>(result.MediaLink);
                }
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<string>(ex.Message);
            }
        }

        public async Task<Result<string>> DeleteImageAsync(string fileName)
        {
            try
            {
                await storageClient.DeleteObjectAsync(googleCloudSettings.GoogleCloudStorageBucket, fileName);
                return new SuccessResult<string>($"Deleted {fileName}");
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<string>(ex.Message);
            }
        }
    }
}