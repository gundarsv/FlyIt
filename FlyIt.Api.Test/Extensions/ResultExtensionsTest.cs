using FlyIt.Api.Extensions;
using FlyIt.Api.Test.Fakes;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyIt.Api.Test.Extensions
{
    public class ResultExtensionsTest
    {
        private readonly FakeController controller;

        public ResultExtensionsTest()
        {
            controller = new FakeController();
        }

        [TestClass]
        public class FromResult : ResultExtensionsTest
        {
            [TestMethod]
            public void ReturnsNoContentResultIfSuccessWithoutData()
            {
                var input = new SuccessResult<string>(null);

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }

            [TestMethod]
            public void ReturnsOkObjectResultIfSuccessWithData()
            {
                var input = new SuccessResult<string>("value");

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(OkObjectResult));

                var resultCast = result as OkObjectResult;

                Assert.IsTrue(resultCast.Value as string == input.Data);
            }

            [TestMethod]
            public void ReturnsNotFoundObjectResultIfNotFound()
            {
                var input = new NotFoundResult<string>("error");

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

                var resultCast = result as NotFoundObjectResult;

                Assert.IsTrue((resultCast.Value as List<string>).FirstOrDefault() == input.Errors.FirstOrDefault());
            }

            [TestMethod]
            public void ReturnsBadRequestObjectResultIfInvalid()
            {
                var input = new InvalidResult<string>("error");

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

                var resultCast = result as BadRequestObjectResult;

                Assert.IsTrue((resultCast.Value as List<string>).FirstOrDefault() == input.Errors.FirstOrDefault());
            }

            [TestMethod]
            public void ReturnsBadRequestObjectResultIfUnexpected()
            {
                var input = new UnexpectedResult<string>("error");

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

                var resultCast = result as BadRequestObjectResult;

                Assert.IsTrue((resultCast.Value as List<string>).FirstOrDefault() == input.Errors.FirstOrDefault());
            }

            [TestMethod]
            public void ReturnsStatusCodeResultResultIfCreatedWithoutData()
            {
                var input = new CreatedResult<string>(null);

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));

                var resultCast = result as StatusCodeResult;

                Assert.IsTrue(resultCast.StatusCode == 201);
            }

            [TestMethod]
            public void ReturnsOkObjectResultResultIfCreatedWithData()
            {
                var input = new CreatedResult<string>("value");

                var result = controller.FromResult(input);

                Assert.IsInstanceOfType(result, typeof(ObjectResult));

                var resultCast = result as ObjectResult;

                Assert.IsTrue(resultCast.StatusCode == 201);
                Assert.IsTrue(resultCast.Value == resultCast.Value);
            }

            [TestMethod]
            public void ThrowsExceptionIfNull()
            {
                Assert.ThrowsException<NullReferenceException>(() =>
                {
                    controller.FromResult((Result<string>)null);
                });
            }
        }
    }
}