using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.DataAccess.Entities
{
    public class News
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Imageurl { get; set; }

        public string Body { get; set; }

        public int AirportId { get; set; }

        public virtual Airport Airport { get; set; }
    }
}