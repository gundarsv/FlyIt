namespace FlyIt.Domain.Models
{
    public class AirportDTO
    {
        public int Id { get; set; }

        public string Iata { get; set; }

        public string Name { get; set; }

        public string MapUrl { get; set; }

        public string MapName { get; set; }

        public string RentingCompanyUrl { get; set; }

        public string RentingCompanyName { get; set; }

        public int RentingCompanyPhoneNo { get; set; }
    }
}