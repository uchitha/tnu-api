using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace TnuBaseApp.Controllers
{
    public class AusPostController : ApiController
    {

        private readonly string AusPostApiUrl = "https://auspost.com.au/api/postcode/search.json?state=WA&excludePostBoxFlag=true&q={0}";
        private readonly string ApiKey = "0feb62bf-cc1d-4dcc-8ca6-6769163eb6c7"; //"28744ed5982391881611cca6cf5c2409"; 
    
        public IList<string> Get(string term)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(string.Format(AusPostApiUrl, term)),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            request.Headers.Add("AUTH-KEY", ApiKey);
           
            var task = client.SendAsync(request).ContinueWith((taskWithMsg) => 
            {
                var response = taskWithMsg.Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            });

            task.Wait();
            var jsonResult = task.Result;
            //dynamic obj = JsonConvert.DeserializeObject(jsonResult);
            JObject obj = JObject.Parse(jsonResult);


            List<string> locationList = new List<string>();

            var localities = obj["localities"];

            if (localities == null || !localities.HasValues) return locationList; //null when error response is sent from AusPost

            if (localities["locality"].Type ==JTokenType.Object) //1 result
            {
                locationList.Add((string)localities["locality"]["location"]);
                return locationList;
            }

            var list = localities["locality"].ToList();

            foreach (var instance in list)
                {
                    var loc = (string)instance["location"];
                    if (!string.IsNullOrEmpty(loc)) locationList.Add(loc);
                }
           
            return locationList;
        }


        public string GetRaw(string id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(string.Format(AusPostApiUrl, id)),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            request.Headers.Add("AUTH-KEY", ApiKey);

            var task = client.SendAsync(request).ContinueWith((taskWithMsg) =>
            {
                var response = taskWithMsg.Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            });

            task.Wait();
            return task.Result;
        }

    }

    public class AusPostLocation
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string PostCode { get; set; }
         [JsonIgnore]
        public string State { get; set; }
    }
}