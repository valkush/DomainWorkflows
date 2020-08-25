namespace WeatherWidget.Events
{
    public class WeatherResponse
    {
        public string RegionId { get; set; }

        public int Temperature { get; set; }
        public int Wind { get; set; }
        public byte Humidity { get; set; }
    }
}
