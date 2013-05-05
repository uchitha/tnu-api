using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TnuBaseApp.Controllers
{
    public class PowerInterruptionController : ApiController
    {
        //Returns Json
        public string Get(string id)
        {
            var piScraper = new WpPiScraper(id);
            return piScraper.GetInteruptionData();
        }
      
    }

    public class WpPiScraper
    {
        public readonly string _url = "http://www.westernpower.com.au/online/pii/?location={0}";

        public WpPiScraper(string location)
        {
            _url = string.Format(_url, location);
        }

        public string GetInteruptionData()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(LoadPage());

            var details = doc.DocumentNode.SelectNodes("//table/tbody/tr");

            var locationList = new List<Location>();

            if (details != null && details.Count > 0)
            {
                foreach (var interuptionInfoItem in details)
                {
                    var resultLine = interuptionInfoItem.Descendants().Where(n => n.Name.Equals("td")).ToList();
                    var location = new Location()
                    {
                        Name = resultLine[0].InnerHtml,
                        Details = resultLine[1].InnerHtml,
                    };
                    locationList.Add(location);
                }
            }

            return JsonConvert.SerializeObject(locationList);
        }

        private string LoadPage()
        {
            var request = WebRequest.Create(_url);
            var response = request.GetResponse();
            var data = response.GetResponseStream();
            string html = string.Empty;
            using (var sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            return html;

        }

    }

    public class Location
    {
        private readonly string NoInterruptionText = "No known interruptions";

        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsInterrupted { get { return !Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()); } }
    }
}
