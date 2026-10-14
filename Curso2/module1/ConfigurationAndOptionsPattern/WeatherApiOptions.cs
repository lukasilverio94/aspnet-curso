using System.ComponentModel.DataAnnotations;

namespace ConfigurationAndOptionsPattern
{
    public class WeatherApiOptions
    {
        [Required(ErrorMessage = "ApiKey is required")]
        [StringLength(25, MinimumLength = 6)]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        [Url(ErrorMessage = "The provided URL is not valid")]
        [StringLength(5000, MinimumLength = 22)]
        public string? BaseUrl { get; set; }

        [Range(1, 300, ErrorMessage = "Timeout Seconds must be between 1 and 300.")]
        public int TimeoutSeconds { get; set; }

        public bool EnableCaching { get; set; }
    }
}
