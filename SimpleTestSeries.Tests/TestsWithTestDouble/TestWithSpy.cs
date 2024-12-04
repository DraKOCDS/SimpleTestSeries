using Microsoft.AspNetCore.Mvc;
using SimpleTestSeries.Controllers;
using SimpleTestSeries.Services;

namespace SimpleTestSeries.Tests.TestsWithTestDouble;

public class TestWithSpy
{
    private readonly WeatherForecastController _sut;
    private readonly SpyWeatherService _weatherService;
    private readonly string _cityWithData = "Rome";

    public TestWithSpy()
    {
        _weatherService = new SpyWeatherService();
        _sut = new WeatherForecastController(_weatherService, new DummyLogger<WeatherForecastController>());
    }

    [Fact]
    public void Get_ReturnOk_When_Data_Exists()
    {
        IActionResult actual = _sut.Get(_cityWithData);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.Equal(1, _weatherService.NumberOfCall);
        Assert.Equal(_cityWithData, _weatherService.LastRequestedCity);
        Assert.NotEmpty(forecasts);
    }

    [Fact]
    public void Get_ReturnNoContent_When_Data_NotExists()
    {
        IActionResult actual = _sut.Get("Paris");

        Assert.IsType<NoContentResult>(actual);
        Assert.Equal(1, _weatherService.NumberOfCall);
        Assert.Equal("Paris", _weatherService.LastRequestedCity);
    }
}

internal class SpyWeatherService : IWeatherService
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

    public int NumberOfCall = 0;
    public string LastRequestedCity = string.Empty;

    public IEnumerable<WeatherForecast> GetByCity(string city)
    {
        NumberOfCall++;
        LastRequestedCity = city;
        return string.Equals(city, "Rome") ? _weatherForecast : Enumerable.Empty<WeatherForecast>();
    }
}