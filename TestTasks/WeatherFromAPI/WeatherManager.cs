using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TestTasks.WeatherFromAPI.Models;

namespace TestTasks.WeatherFromAPI
{
    public class WeatherManager
    {
        private const string ApiKey = "12064f6fb8cc282107c9f10c4ae5e594";
        private const string GeocodingUrl = "http://api.openweathermap.org/geo/1.0/direct?q={0}&limit=1&appid={1}";
        private const string ForecastUrl = "https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&appid={2}&units=metric";

        private readonly HttpClient _httpClient;

        public WeatherManager() : this(new HttpClient()) { }
        public WeatherManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<WeatherComparisonResult> CompareWeather(string cityA, string cityB, int dayCount)
        {
            if (string.IsNullOrWhiteSpace(cityA) || string.IsNullOrWhiteSpace(cityB) || dayCount < 1 || dayCount > 5)
                throw new ArgumentException("Invalid input parameters.");

            var coordsA = await GetCoordinates(cityA);
            var coordsB = await GetCoordinates(cityB);

            var forecastA = await GetForecast(coordsA);
            var forecastB = await GetForecast(coordsB);

            int warmerDays = 0, rainierDays = 0;

            for (int i = 0; i < dayCount; i++)
            {
                var date = DateTime.UtcNow.Date.AddDays(i);
                var dayDataA = forecastA.List.Where(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).UtcDateTime.Date == date);
                var dayDataB = forecastB.List.Where(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).UtcDateTime.Date == date);

                if (!dayDataA.Any() || !dayDataB.Any()) continue;

                var avgTempA = dayDataA.Average(f => f.Temp);
                Console.WriteLine(avgTempA);
                var avgTempB = dayDataB.Average(f => f.Temp);
                Console.WriteLine(avgTempB);
                if (avgTempA > avgTempB) warmerDays++;

                var totalRainA = dayDataA.Sum(f => f.Rain);
                var totalRainB = dayDataB.Sum(f => f.Rain);
                if (totalRainA > totalRainB) rainierDays++;
            }

            return new WeatherComparisonResult(cityA, cityB, warmerDays, rainierDays);
        }

        private async Task<Geolocation> GetCoordinates(string city)
        {
            var response = await _httpClient.GetStringAsync(string.Format(GeocodingUrl, city, ApiKey));

            var locations = JsonSerializer.Deserialize<List<Geolocation>>(response);

            if (locations == null) throw new ArgumentException($"City '{city}' not found.");
            return locations.First(); 
        }

        private async Task<WeatherForecastList> GetForecast(Geolocation geolocation)
        {
            var weatherResponse = await _httpClient.GetStringAsync(string.Format(ForecastUrl, geolocation.Lat, geolocation.Lon, ApiKey));
            var forecastList = JsonSerializer.Deserialize<WeatherForecastList>(weatherResponse);
            
            return forecastList;
        }

        public class Geolocation
        {
            [JsonPropertyName("lat")]
            public double Lat { get; set; }
            [JsonPropertyName("lon")]
            public double Lon { get; set; }
        }
        public class WeatherForecastList
        {
            [JsonPropertyName("list")]
            public List<WeatherForecast> List { get; set; }
        }
        public class WeatherForecast
        {
            [JsonPropertyName("dt")]
            public long Dt { get; set; }
            [JsonPropertyName("main")]
            public MainData MainData { get; set; }
            [JsonPropertyName("rain")]
            public RainData RainData { get; set; }
            public double Rain => RainData?.Volume ?? 0;
            public double Temp => MainData.Temp;
        }

        public class MainData
        {
            [JsonPropertyName("temp")]
            public double Temp { get; set; }
        }

        public class RainData
        {
            [JsonPropertyName("3h")]
            public double Volume { get; set; }
        }
    }
}
