namespace SimpleTestSeries.Services
{
    public interface IWeatherService
    {
        IEnumerable<WeatherForecast> GetByCity(string city);
    }
}