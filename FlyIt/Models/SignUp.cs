using System.ComponentModel.DataAnnotations;

namespace FlyIt.Api.Models
{
    public class SignUp
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
