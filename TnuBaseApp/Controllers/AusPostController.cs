using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace TnuBaseApp.Controllers
{
    public class AusPostController : ApiController
    {

        private readonly string Url = "https://test.npe.auspost.com.au/search?q="; 
        private readonly string ApiKey = "28744ed5982391881611cca6cf5c2409"; 
    
        public string Get(string id)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(Url + id),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
            request.Headers.Add("AUTH-KEY", ApiKey);

            var task = client.SendAsync(request).ContinueWith((taskWithMsg) => 
            {
                var response = taskWithMsg.Result;
                return response.Content;
            });

            task.Wait();
            return string.Empty;
        }
        
    }
}