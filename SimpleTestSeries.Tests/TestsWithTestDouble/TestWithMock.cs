using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleTestSeries.Controllers;
using SimpleTestSeries.Services;

namespace SimpleTestSeries.Tests.TestsWithTestDouble;

public class TestWithMoq
{
    private readonly WeatherForecastController _sut;
    private readonly Mock<IWeatherService> _weatherService;
    private readonly string _cityWithData = "Rome";

    public TestWithMoq()
    {
        _weatherService = new Mock<IWeatherService>();
        _sut = new WeatherForecastController(_weatherService.Object, new DummyLogger<WeatherForecastController>());
    }

    [Fact]
    public void Get_ReturnOk_When_Data_Exists()
    {
        List<WeatherForecast> expectedForecast = [new WeatherForecast()];
        _weatherService.Setup(x => x.GetByCity(_cityWithData))
            .Returns(expectedForecast)
            .Verifiable(Times.Once);

        IActionResult actual = _sut.Get(_cityWithData);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.NotEmpty(forecasts);
        Assert.Same(expectedForecast, forecasts);
        _weatherService.Verify();
    }

    [Fact]
    public void Get_ReturnNoContent_When_Data_NotExists()
    {
        IActionResult actual = _sut.Get("Paris");

        Assert.IsType<NoContentResult>(actual);
    }
}