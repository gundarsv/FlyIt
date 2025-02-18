﻿using System.Collections.Generic;

namespace FlyIt.Domain.ServiceResult
{
    public class NotFoundResult<T> : Result<T>
    {
        private readonly string _error;

        public NotFoundResult(string error)
        {
            _error = error;
        }

        public override ResultType ResultType => ResultType.NotFound;

        public override List<string> Errors => new List<string> { _error ?? "The entity you're looking for cannot be found" };

        public override T Data => default(T);
    }
}