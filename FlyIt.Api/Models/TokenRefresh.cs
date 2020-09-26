using System.ComponentModel.DataAnnotations;

namespace FlyIt.Api.Models
{
    public class TokenRefresh
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
