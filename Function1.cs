using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Extensibility;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ACDC_PollutionAF
{
    public static class Function1
    {
        [FunctionName("PollutionAlert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("PollutionAlert function processed a request.");

            //string dataIn = req.Query["data"];
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic convertedData = JsonConvert.DeserializeObject(requestBody);
            //string[] values = String.split(convertedData, ","); ?

            //mockdata
            var data_loc = "Oslofjorden";
            var data_lat = 59.12387;
            var data_long = 10.10293;
            string responseMessage = "Yo mama's so fat, when she skips a meal, the stock market drops.";

            //event,latitude,longitude,sound(db),pir-direction,soil-moisture,temperature,timestamp,ultrasonic-distance,uv-level,dust-level,waterlevel
            //Todo split that shit

            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"Lokasjon", data_loc},
                {"Latitude", data_lat.ToString()},
                {"Longitude", data_long.ToString()}
           };
            var outData = JsonConvert.SerializeObject(dict);
            
            var url = Environment.GetEnvironmentVariable("GetFlowUrl", EnvironmentVariableTarget.Process);

            var client = new HttpClient { BaseAddress = new Uri(url) };

            HttpRequestMessage reqmes = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            
            reqmes.Content = new StringContent(JsonConvert.SerializeObject(outData), UTF8Encoding.UTF8, "application/json");
            reqmes.Headers.Add("Accept", "application/json");
            reqmes.Method = HttpMethod.Post;
            
            HttpResponseMessage response = await client.SendAsync(reqmes);
            log.LogInformation(await response.Content.ReadAsStringAsync());

            return new OkObjectResult(responseMessage);
        }
    }
}
