using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using DomainWorkflows.Workflows;

using WeatherWidget.Events;
using WeatherWidget.Services;

namespace WeatherWidget.Workflows
{
    public class WeatherWorkflow : Workflow
    {        
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherWorkflow> _logger;

        [Persist]
        private string _regionId;

        [Persist]
        private WeatherData _currentWeather = null; // weather cache for _regionId

        [Persist]
        private int _refreshCount; // refresh slider - we also can use schedule slider here

        public WeatherWorkflow(IWeatherService weatherService, ILogger<WeatherWorkflow> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        public async Task<WeatherResponse> Handle(WeatherRequest request)
        {
            _regionId = request.RegionId;

            _logger.LogInformation("Weather requested for {RegionId} region", _regionId);

            if (_currentWeather == null || _refreshCount >= 10)
            {
                _currentWeather = await _weatherService.GetWeather(_regionId);
                _logger.LogInformation("Weather is loaded for {RegionId} region", _regionId);
            }
            else
            {
                _logger.LogWarning("Weather is taken from cache for {RegionId} region", _regionId);
            }

            // refresh weather 10 times every 20 sec after this request to keep valid cache
            _refreshCount = 0;            

            return Response(_currentWeather);
        }

        [Repeat("20 sec", State = "_refreshCount < 10")]
        public async Task RefreshWeather()
        {
            _currentWeather = await _weatherService.GetWeather(_regionId);

            _logger.LogInformation("Weather is refreshed for {RegionId} region", _regionId);

            _refreshCount++;            
        }        

        private WeatherResponse Response(WeatherData weather)
        {
            return new WeatherResponse()
            {   
                RegionId = _regionId, 
                
                Temperature = weather.Temperature,
                Wind = weather.Wind,
                Humidity = weather.Humidity
            };
        }
    }
}
