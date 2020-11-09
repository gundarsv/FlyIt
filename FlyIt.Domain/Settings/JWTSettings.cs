namespace FlyIt.Domain.Settings
{
    public class JWTSettings
    {
        public string Issuer { get; set; }

        public string Secret { get; set; }

        public int RefreshTokenExpirationInDays { get; set; }

        public int AccessTokenExpirationInDays { get; set; }
    }
}