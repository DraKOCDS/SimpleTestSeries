namespace SimpleTestSeries.Services
{
    public class WeatherService : IWeatherService
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private static readonly string[] Cities =
        [
            "Rome", "London", "Berlin", "Madrid", "Paris"
        ];

        private readonly Dictionary<string, IEnumerable<WeatherForecast>> _forecast;

        public WeatherService()
        {
            _forecast = [];
            CreateForecast();
        }

        private void CreateForecast()
        {
            foreach (var city in Cities)
            {
                var forcast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray();

                _forecast.Add(city, forcast);
            }
        }

        public IEnumerable<WeatherForecast> GetByCity(string city)
            => _forecast.TryGetValue(city, out IEnumerable<WeatherForecast>? value) ? value : [];

    }
}