using FlyIt.Domain.Models.MetarResponse;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface ICheckWXAPIMetarService
    {
        public Task<MetarResponse> GetMetarByICAO(string icao);
    }
}