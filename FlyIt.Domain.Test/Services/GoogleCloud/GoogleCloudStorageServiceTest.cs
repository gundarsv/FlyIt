using FlyIt.Domain.ServiceResult;
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
        public class UploadImageAsync : GoogleCloudStorageServiceTest
        {
            [TestMethod]
            public async Task ReturnsInvalidIfImageFileIsNull()
            {
                var result = await googleCloudStorageService.UploadImageAsync(null);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotUploaded()
            {
                var mockFormFile = new Mock<IFormFile>();

                mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

                storageClient.Setup(x => x.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null)).ReturnsAsync((Google.Apis.Storage.v1.Data.Object)null);

                var result = await googleCloudStorageService.UploadImageAsync(mockFormFile.Object);

                storageClient.Verify(m => m.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUploaded()
            {
                var mockFormFile = new Mock<IFormFile>();

                mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

                var objectToReturn = new Google.Apis.Storage.v1.Data.Object()
                {
                    MediaLink = "MediaLink"
                };

                storageClient.Setup(x => x.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null)).ReturnsAsync(objectToReturn);

                var result = await googleCloudStorageService.UploadImageAsync(mockFormFile.Object);

                storageClient.Verify(m => m.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(objectToReturn.MediaLink, result.Data.Url);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                var mockFormFile = new Mock<IFormFile>();

                mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

                storageClient.Setup(x => x.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null)).Throws(new Exception());

                var result = await googleCloudStorageService.UploadImageAsync(mockFormFile.Object);

                storageClient.Verify(m => m.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), null, default, null), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteImageAsync : GoogleCloudStorageServiceTest
        {
            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                storageClient.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default)).Throws(new Exception());

                var result = await googleCloudStorageService.DeleteImageAsync(It.IsAny<string>());

                storageClient.Verify(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfDeleted()
            {
                storageClient.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default)).Returns(Task.CompletedTask);

                var result = await googleCloudStorageService.DeleteImageAsync(It.IsAny<string>());

                storageClient.Verify(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), null, default), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }
    }
}