namespace CleanArch.Domain.Services.Configuration
{
    public class CORS
    {
        public bool AllowAnyOrigin { get; set; }
        public string[] AllowedOrigins { get; set; }
    }
}
