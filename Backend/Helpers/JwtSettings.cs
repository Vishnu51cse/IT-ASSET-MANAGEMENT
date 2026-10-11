namespace Backend.Helpers
{
    public class JwtSettings
    {
        public string Secret { get; set; } = "ITAssetManagement_Super_Secret_JWT_Key_2026_Portfolio_Project!";
        public string Issuer { get; set; } = "ITAssetManagementAPI";
        public string Audience { get; set; } = "ITAssetManagementApp";
        public int ExpiryInHours { get; set; } = 24;
    }
}
