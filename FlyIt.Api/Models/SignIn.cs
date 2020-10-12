using System.ComponentModel.DataAnnotations;

namespace FlyIt.Api.Models
{
    public class SignIn
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}