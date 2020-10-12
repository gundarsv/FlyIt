using FlyIt.Domain.Settings;
using Google.Cloud.Storage.V1;

namespace FlyIt.Domain.Services
{
    public interface IStorageClientWrapper
    {
        public StorageClient GetStorageClient(GoogleCloudSettings googleCloudSettings);
    }
}
