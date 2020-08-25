using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherWidget.Services
{
    internal sealed class WeatherService : IWeatherService
    {
        private Random _rand = new Random();

        private Dictionary<string, WeatherData> _storage = new Dictionary<string, WeatherData>()
        {
            { "1", new WeatherData() { Temperature=18, Wind=3, Humidity=62 } },
            { "2", new WeatherData() { Temperature=15, Wind=5, Humidity=52 } },
            { "3", new WeatherData() { Temperature=32, Wind=2, Humidity=60 } },
            { "4", new WeatherData() { Temperature=18, Wind=7, Humidity=51 } },
            { "5", new WeatherData() { Temperature=10, Wind=8, Humidity=76 } }
        };

        public async Task<WeatherData> GetWeather(string regionId)
        {
            await Task.Delay(300);

            if (_storage.TryGetValue(regionId, out WeatherData data))
            {
                ChangeWeatherRandomly(data);                

                return data;
            }
            else
            {
                throw new InvalidOperationException($"Invalid region id \"{regionId}\"");
            }
        }

        private void ChangeWeatherRandomly(WeatherData data)
        {
            WeatherChange change = GetRandomWeatherChange();

            data.Temperature += change.TemperatureDelta;

            data.Wind += change.WindDelta;
            if (data.Wind < 0) data.Wind = 0;

            data.Humidity = (byte)(data.Humidity + change.HumidityDelta);
            if (data.Humidity < 20) data.Humidity = 20;
            if (data.Humidity > 100) data.Humidity = 100;
        }

        private WeatherChange GetRandomWeatherChange()
        {
            int tempDelta = 0;
            if (_rand.Next(100) > 35)
            {
                int rand = _rand.Next(100);
                if (rand < 40) tempDelta = 1;
                else if (rand < 60) tempDelta = -1;
                else if (rand < 70) tempDelta = 2;
                else if (rand < 80) tempDelta = -2;
            }

            int windDelta = 0;
            if (_rand.Next(100) > 45)
            {
                int rand = _rand.Next(100);
                if (rand < 30) windDelta = 1;
                else if (rand < 60) windDelta = -1;
                else if (rand < 70) windDelta = 2;
                else if (rand < 80) windDelta = -2;
            }

            int humDelta = 0;
            if (_rand.Next(100) > 60)
            {
                int rand = _rand.Next(100);
                if (rand < 45) humDelta = 2;
                else if (rand < 80) humDelta = -2;
                else if (rand < 90) humDelta = 4;
                else if (rand < 95) humDelta = -4;
                else if (rand < 100) humDelta = 5;
            }

            return new WeatherChange()
            {
                TemperatureDelta = tempDelta,
                WindDelta = windDelta,
                HumidityDelta = humDelta
            };
        }

        private class WeatherChange
        {
            public int TemperatureDelta { get; set; }
            public int WindDelta { get; set; }
            public int HumidityDelta { get; set; }
        }
    }
}
