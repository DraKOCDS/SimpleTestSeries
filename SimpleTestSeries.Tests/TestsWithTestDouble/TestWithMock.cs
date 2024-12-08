using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using SimpleTestSeries.Controllers;
using SimpleTestSeries.Services;

namespace SimpleTestSeries.Tests.TestsWithTestDouble;

public class TestWithMock
{
    private readonly WeatherForecastController _sut;
    private readonly MockWeatherService _weatherService;
    private readonly string _cityWithData = "Rome";

    public TestWithMock()
    {
        _weatherService = new MockWeatherService();
        _sut = new WeatherForecastController(_weatherService, new DummyLogger<WeatherForecastController>());
    }

    [Fact]
    public void Get_ReturnOk_When_Data_Exists()
    {
        List<WeatherForecast> expectedForecast = [
            new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = 20,
                Summary = "Test Summary"
            }
        ];

        _weatherService.SetUpGetByCity(_cityWithData, expectedForecast);
        IActionResult actual = _sut.Get(_cityWithData);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.NotEmpty(forecasts);
        Assert.Same(expectedForecast, forecasts);

        _weatherService.VerifyGetByCity(_cityWithData);
    }

    [Fact]
    public void Get_ReturnOk_When_Data_Exists2()
    {
        List<WeatherForecast> expectedForecast = [
            new WeatherForecast()
        ];

        _weatherService.SetUpGetByCity(_cityWithData, expectedForecast);
        IActionResult actual = _sut.Get(_cityWithData);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);

        Assert.Same(expectedForecast, forecasts);

        _weatherService.VerifyGetByCity(_cityWithData);
    }

    [Fact]
    public void Get_ReturnOk_With_AutoFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(c => c.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));

        List<WeatherForecast> expectedForecast = fixture.CreateMany<WeatherForecast>().ToList();

        _weatherService.SetUpGetByCity(_cityWithData, expectedForecast);
        IActionResult actual = _sut.Get(_cityWithData);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);

        Assert.Same(expectedForecast, forecasts);

        _weatherService.VerifyGetByCity(_cityWithData);
    }


    [Fact]
    public void Get_ReturnNoContent_When_Data_NotExists()
    {
        IActionResult actual = _sut.Get("Paris");

        Assert.IsType<NoContentResult>(actual);
        _weatherService.VerifyGetByCity("Paris");
    }
}

internal class MockWeatherService : IWeatherService
{
    private readonly Dictionary<string, List<WeatherForecast>> _expectedResponses = [];
    private readonly List<string> _calledWithCities = [];

    public void SetUpGetByCity(string city, List<WeatherForecast> weatherForecasts)
        => _expectedResponses[city] = weatherForecasts;

    public void VerifyGetByCity(string cityToVerify, int times = 1)
    {
        var actualCount = _calledWithCities.Count(x => x == cityToVerify);
        if (actualCount != times)
            throw new Exception($"GetByCity to be called {times} time(s) with city {cityToVerify}, but was called {actualCount} time(s).");
    }

    public IEnumerable<WeatherForecast> GetByCity(string city)
    {
        _calledWithCities.Add(city);
        return _expectedResponses.TryGetValue(city, out List<WeatherForecast>? value) ? value : Enumerable.Empty<WeatherForecast>();
    }
}