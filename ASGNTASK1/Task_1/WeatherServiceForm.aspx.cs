using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Task_1
{
    public partial class WeatherServiceForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetData();
        }

        public string GetData()
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
                        weatherDetails.InnerText = result;
                    }
                    else
                    {
                        Response.ContentType = "text/html";
                        Response.Write("<h2> Error accessing web service </h2>");
                    }
                }
                return "Success";
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }
    }
}