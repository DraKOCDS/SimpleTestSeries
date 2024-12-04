using Microsoft.AspNetCore.Mvc;
using SimpleTestSeries.Controllers;
using SimpleTestSeries.Services;

namespace SimpleTestSeries.Tests.TestsWithTestDouble;

public class TestsWithStub
{
    private readonly WeatherForecastController _sut;
    private readonly string _cityWithData = "Rome";

    public TestsWithStub()
    {
        _sut = new WeatherForecastController(new StubWeatherService(), new DummyLogger<WeatherForecastController>());
    }

    [Fact]
    public void Get_ReturnOk_When_Data_Exists()
    {
        IActionResult actual = _sut.Get(_cityWithData);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.NotEmpty(forecasts);
    }

    [Fact]
    public void Get_ReturnNoContent_When_Data_NotExists()
    {
        IActionResult actual = _sut.Get("Paris");

        Assert.IsType<NoContentResult>(actual);
    }
}

internal class StubWeatherService : IWeatherService
{
    private readonly List<WeatherForecast> _weatherForecast = 
        [
            new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = 20,
                Summary = "Test Summary"
            }
        ];

    public IEnumerable<WeatherForecast> GetByCity(string city)
        => string.Equals(city, "Rome") ? _weatherForecast : Enumerable.Empty<WeatherForecast>();
}
