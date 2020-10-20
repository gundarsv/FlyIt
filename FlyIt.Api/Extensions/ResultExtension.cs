using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FlyIt.Api.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
        {
            switch (result.ResultType)
            {
                case ResultType.Ok:
                    if (result.Data is null)
                        return controller.NoContent();
                    else
                        return controller.Ok(result.Data);

                case ResultType.NotFound:
                    return controller.NotFound(result.Errors);

                case ResultType.Invalid:
                    return controller.BadRequest(result.Errors);

                case ResultType.Unexpected:
                    return controller.BadRequest(result.Errors);

                case ResultType.Unauthorized:
                    return controller.Unauthorized();

                case ResultType.Created:
                    if (result.Data is null)
                    {
                        return controller.StatusCode(201);
                    }
                    return controller.StatusCode(201, result.Data);

                default:
                    throw new Exception("An unhandled result has occurred as a result of a service call.");
            }
        }
    }
}