using FlyIt.Domain.Settings;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Text.Json;

namespace FlyIt.Domain.Services
{
    public class StorageClientWrapper : StorageClient, IStorageClientWrapper
    {
        public StorageClient GetStorageClient(GoogleCloudSettings googleCloudSettings)
        {
            var googleCredentials = GoogleCredential.FromJson(JsonSerializer.Serialize(googleCloudSettings.GoogleCloudKey));

            return Create(googleCredentials);
        }
    }
}
