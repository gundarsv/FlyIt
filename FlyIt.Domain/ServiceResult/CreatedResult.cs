using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.Domain.ServiceResult
{
    public class CreatedResult<T> : Result<T>
    {
        private readonly T _data;
        public CreatedResult(T data)
        {
            _data = data;
        }
        public override ResultType ResultType => ResultType.Created;

        public override List<string> Errors => new List<string>();

        public override T Data => _data;
    }
}
