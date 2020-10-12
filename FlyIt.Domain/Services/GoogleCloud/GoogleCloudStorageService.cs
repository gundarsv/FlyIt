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

        public async Task<string> UploadImageAsync(IFormFile imageFile, string fileName)
        {
            try
            {
                if (imageFile is null)
                {
                    return null;
                }

                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);

                    var result = await storageClient.UploadObjectAsync(googleCloudSettings.GoogleCloudStorageBucket, fileName, imageFile.ContentType, memoryStream);

                    return result.MediaLink;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public async Task<string> DeleteImageAsync(string fileName)
        {
            try
            {
                await storageClient.DeleteObjectAsync(googleCloudSettings.GoogleCloudStorageBucket, fileName);
                return $"Deleted {fileName}";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
