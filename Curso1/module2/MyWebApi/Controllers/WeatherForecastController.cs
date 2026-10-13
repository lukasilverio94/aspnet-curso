using Microsoft.AspNetCore.Mvc;

namespace MyWebApi.Controllers
{

    [ApiController]

    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase

    {

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                rng.Next(-20, 55),
                Summaries[rng.Next(Summaries.Length)]
            )).ToArray();

        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            return Ok(forecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WeatherForecast forecast)
        {
            // Update data for the given ID
            // Example: Find and update an item with a matching ID
            var existingForecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 0, null); // Replace with actual data retrieval logic
            existingForecast.Date = forecast.Date;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Delete data for the given ID
            return NoContent();
        }
    }
}

