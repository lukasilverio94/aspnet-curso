using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigurationAndOptionsPattern.Controllers

{

    [ApiController]

    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherApiOptions _weatherOptions;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherApiOptions weatherApiOptions)
        {
            _logger = logger;
            _weatherOptions = weatherApiOptions;
        }

        // method to get those environments
        [HttpGet("config")]
        public IActionResult GetConfiguration()
        {
            return Ok((new
            {
                BaseUrl = _weatherOptions.BaseUrl,
                TimeoutSeconds = _weatherOptions.TimeoutSeconds,
                Enablecaching = _weatherOptions.EnableCaching,
                HasApiKey = !string.IsNullOrEmpty(_weatherOptions.ApiKey)
            }));
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Message = "Weather service is configured and running",
                Configured = !string.IsNullOrEmpty(_weatherOptions.ApiKey),
                CachingEnabled = _weatherOptions.EnableCaching
            });
        }
    }

}