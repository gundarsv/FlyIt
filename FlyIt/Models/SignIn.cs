using System.ComponentModel.DataAnnotations;

namespace FlyIt.Api.Models
{
    public class SignIn
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
