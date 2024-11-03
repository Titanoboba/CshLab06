using System;
using System.Text.Json;

struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }

    public Weather(string country, string name, double temp, string description)
    {
        Country = country;
        Name = name;
        Temp = temp;
        Description = description;
    }


    public override string ToString()
    {
        return Country;
    }

}

public class Programm
{
    private static HttpClient client = new HttpClient();
    private const string api = "58c7e3310fab827cf68e40de14dc765d";

    private static async Task<Weather> getRandomWeather(double latitude, double longtitude)
    {
        string sendRequest = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longtitude}&appid={api}";
        
        Console.WriteLine($"{sendRequest} Sent url");
        
        var getRequest = await client.GetStringAsync(sendRequest);

        Console.WriteLine($"Got request {getRequest}");

        JsonDocument rawInfo = JsonDocument.Parse(getRequest);
        var root = rawInfo.RootElement;
        if (root.TryGetProperty("sys", out JsonElement country) && root.TryGetProperty("name", out JsonElement name))
        {
            if (name.ToString() == "" || name.ToString() == null || country.ToString() == "" || country.ToString() == null)
            {
                return default;
            }
            root.TryGetProperty("main", out JsonElement temp);
            double tempStruct = temp.GetProperty("temp").GetDouble();
            root.TryGetProperty("weather", out JsonElement weather);
            string description = weather[0].GetProperty("description").ToString();
            Weather result = new Weather(country.ToString(), name.ToString(), tempStruct, description);
            Console.WriteLine($"Going to remember this country, it's name is {name}");
            return result; 
        }
        return default;
    }

    public static async Task Main()
    {

        List<Weather> weather = new List<Weather>();
        Random rnd = new Random();
        while (weather.Count < 50)
        {
            double latitude = rnd.NextDouble() * 180 - 90;
            double longtitude = rnd.NextDouble() * 360 - 180;
            Console.WriteLine($"{weather.Count + 1}: Send request (lat {latitude}, lon {longtitude})");
            Weather newWeather = await getRandomWeather(latitude, longtitude);
            Console.WriteLine($"{weather.Count + 1}: Got request");
            if (newWeather.Name != "" && newWeather.Name != null && newWeather.Country != null && newWeather.Country != "")
            {
                weather.Add(newWeather);
            }

        }

        int counter_checker = 1;

        foreach (var elem in weather)
        {
            Console.WriteLine($"{counter_checker}: {elem} Name: {elem.Name}");
            counter_checker++;
        }

        var maxTemperature = weather.Max(elem => elem.Temp);
        var minTemperature = weather.Min(elem => elem.Temp);
        var averageTemp = weather.Average(elem => elem.Temp);
        var amountOfCountries = weather.Select(elem => elem.Country).Distinct().Count();
        var clearSkyCountry = weather.FirstOrDefault(elem => elem.Description == "clear sky");
        var rainCountry = weather.FirstOrDefault(elem => elem.Description == "rain");
        var fewCloudsCountry = weather.FirstOrDefault(elem => elem.Description == "few clouds");

        Console.WriteLine(maxTemperature.ToString());
        Console.WriteLine(minTemperature.ToString());
        Console.WriteLine(averageTemp.ToString());
        Console.WriteLine(amountOfCountries.ToString());

        Console.WriteLine($"Clear sky right now is in: {clearSkyCountry.Name}");
        Console.WriteLine($"Rain right now is in: {rainCountry.Name}");
        Console.WriteLine($"Few clouds right now is in: {fewCloudsCountry.Name}");
    }
}