using System.Threading.Tasks;

namespace WeatherWidget.Services
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeather(string regionId);
    }
}
