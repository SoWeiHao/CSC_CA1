using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Task_1.Controllers
{
    public class WeatherDetailsController : ApiController
    {
        [HttpGet]
        [Route("api/weatherDetails")]
        public string WeatherDetails()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("https://api.data.gov.sg/v1/environment/2-hour-weather-forecast").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        return result;
                    }
                    else
                    {
                        return "Error accessing web service";
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }
    }
}
