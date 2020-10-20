using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.Domain.Models
{
    public class NewsDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Imageurl { get; set; }

        public string ImageName { get; set; }

        public string Body { get; set; }

        public AirportDTO Airport { get; set; }
    }
}