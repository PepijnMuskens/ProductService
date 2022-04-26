namespace ProductService
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }
        public string? Summary { get; set; }

        public WeatherForecast(DateTime date, int tempc, string sum)
        {
            Date = date;
            TemperatureC = tempc;
            Summary = sum;
        }
    }
}