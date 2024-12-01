using Microsoft.AspNetCore.Mvc;
using SimpleTestSeries.Services;

namespace SimpleTestSeries.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherForecastController(IWeatherService weatherService, ILogger<WeatherForecastController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet("{city}")]
        public IActionResult Get(string city)
        {
            var data = _weatherService.GetByCity(city);
            if (data.Any())
                return Ok(data);

            _logger.LogInformation("No data found for {City}", city);

            return NoContent();
        }
    }
}
