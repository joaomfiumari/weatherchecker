﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WeatherChecker.Controllers
{
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        [HttpGet("[action]/{city}")]
        public async Task<IActionResult> City(string city)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new System.Uri("http://api.openweathermap.org");
                    var response = await client.GetAsync($"/data/2.5/weather?q={city}&appid=19efe6c141405b18271915691812e9fe&units=metric");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);
                    return Ok(new
                    {
                        Temp = rawWeather.Main.Temp,
                        Summary = string.Join(",", rawWeather.Weather.Select(x => x.Main)),
                        City = rawWeather.Name
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
            }
        }

    }
    public class OpenWeatherResponse
    {
        public string Name { get; set; }

        public IEnumerable<WeatherDescription> Weather { get; set; }

        public Main Main { get; set; }
    }
    public class WeatherDescription
    {
        public string Main { get; set; }
        public string Description { get; set; }
    }

    public class Main
    {
        public string Temp { get; set; }
    }
}