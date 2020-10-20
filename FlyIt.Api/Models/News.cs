using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.Api.Models
{
    public class News
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Imageurl { get; set; }

        [Required]
        public string Body { get; set; }
    }
}