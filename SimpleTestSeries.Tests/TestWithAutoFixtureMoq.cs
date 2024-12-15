using AutoFixture.AutoMoq;
using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleTestSeries.Controllers;
using SimpleTestSeries.Services;
using AutoFixture.Kernel;

namespace SimpleTestSeries.Tests;

public class TestWithAutoFixtureMoq
{
    private readonly IFixture _fixture;

    public TestWithAutoFixtureMoq()
    {
        _fixture = new Fixture().Customize(new CompositeCustomization(
                new AutoMoqCustomization(),
                new ConstructorCustomization(typeof(WeatherForecastController), new GreedyConstructorQuery())
            ));
        _fixture.Customize<DateOnly>(c => c.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
    }

    [Fact]
    public void Get_ReturnOk_When_Data_Exists()
    {
        var expectedForecasts = _fixture.CreateMany<WeatherForecast>().ToList();
        var city = _fixture.Create<string>();

        var weatherService = _fixture.Freeze<Mock<IWeatherService>>();
        weatherService.Setup(x => x.GetByCity(city))
            .Returns(expectedForecasts)
            .Verifiable(Times.Once);

        var sut = _fixture.Create<WeatherForecastController>();

        IActionResult actual = sut.Get(city);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.NotEmpty(forecasts);
        Assert.Same(expectedForecasts, forecasts);
        weatherService.Verify();
    }

    [Theory]
    [AutoData]
    public void Get_ReturnOk_With_AutoData(string city, List<WeatherForecast> expectedForecasts)
    {
        var weatherService = _fixture.Freeze<Mock<IWeatherService>>();
        weatherService.Setup(x => x.GetByCity(city))
            .Returns(expectedForecasts)
            .Verifiable(Times.Once);

        var sut = _fixture.Create<WeatherForecastController>();

        IActionResult actual = sut.Get(city);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.Same(expectedForecasts, forecasts);
        weatherService.Verify();
    }

    [Theory]
    [AutoMoqData]
    public void Get_ReturnOk_With_AutoMoqData(
        [Frozen] Mock<IWeatherService> weatherService, 
        [Greedy] WeatherForecastController sut, 
        string city,
        List<WeatherForecast> expectedForecasts)
    {
        weatherService.Setup(x => x.GetByCity(city))
            .Returns(expectedForecasts)
            .Verifiable(Times.Once);

        IActionResult actual = sut.Get(city);

        var okResult = Assert.IsType<OkObjectResult>(actual);
        var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(okResult.Value);
        Assert.Same(expectedForecasts, forecasts);
        weatherService.Verify();
    }


    [Theory]
    [AutoMoqData]
    public void Get_ReturnNoContent_When_Data_NotExists([Greedy] WeatherForecastController sut)
    {
        IActionResult actual = sut.Get("Paris");

        Assert.IsType<NoContentResult>(actual);
    }
}