using System.Collections.Generic;

namespace FlyIt.Domain.Models
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public List<AirportDTO> Airports { get; set; }
    }
}