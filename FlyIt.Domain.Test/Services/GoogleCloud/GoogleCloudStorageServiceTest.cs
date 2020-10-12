using FlyIt.Domain.Services;
using FlyIt.Domain.Settings;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class GoogleCloudStorageServiceTest
    {
        private readonly Mock<IOptionsSnapshot<GoogleCloudSettings>> googleCloudSettings;
        private readonly Mock<StorageClient> storageClient;
        private readonly Mock<IStorageClientWrapper> storageClientWrapper;
        private readonly GoogleCloudStorageService googleCloudStorageService;

        public GoogleCloudStorageServiceTest()
        {
            storageClient = new Mock<StorageClient>();
            googleCloudSettings = new Mock<IOptionsSnapshot<GoogleCloudSettings>>();
            storageClientWrapper = new Mock<IStorageClientWrapper>();

            googleCloudSettings.Setup(x => x.Value).Returns(new GoogleCloudSettings());
            storageClientWrapper.Setup(x => x.GetStorageClient(It.IsAny<GoogleCloudSettings>())).Returns(storageClient.Object);            

            googleCloudStorageService = new GoogleCloudStorageService(googleCloudSettings.Object, storageClientWrapper.Object);
        }

        [TestClass]
        public class UploadImageAsync: GoogleCloudStorageServiceTest
        {
            [TestMethod]
            public async Task ReturnsNull()
            {
                var result = await googleCloudStorageService.UploadImageAsync(null, null);

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsString()
            {
                var mockFormFile = new Mock<IFormFile>();

                mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

                var objectToReturn = new Google.Apis.Storage.v1.Data.Object() { 
                    MediaLink = "MediaLink"
                };

                storageClient.Setup(x => x.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null)).ReturnsAsync(objectToReturn);

                var result = await googleCloudStorageService.UploadImageAsync(mockFormFile.Object, null);

                storageClient.Verify(m => m.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null), Times.Once);
                Assert.IsNotNull(result);
                Assert.AreEqual(objectToReturn.MediaLink, result);
            }

            [TestMethod]
            public async Task ReturnsNullIfThrowsException()
            {
                var mockFormFile = new Mock<IFormFile>();

                mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

                storageClient.Setup(x => x.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null)).Throws(new Exception());

                var result = await googleCloudStorageService.UploadImageAsync(mockFormFile.Object, null);

                storageClient.Verify(m => m.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null), Times.Once);
                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class DeleteImageAsync : GoogleCloudStorageServiceTest
        {
            [TestMethod]
            public async Task ReturnsNull()
            {
                storageClient.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default)).Throws(new Exception());

                var result = await googleCloudStorageService.DeleteImageAsync(It.IsAny<string>());

                storageClient.Verify(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default), Times.Once);
                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsString()
            {
                storageClient.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default)).Returns(Task.CompletedTask);

                var result = await googleCloudStorageService.DeleteImageAsync(It.IsAny<string>());

                storageClient.Verify(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default), Times.Once);
                Assert.IsNotNull(result);
            }
        }
    }
}
