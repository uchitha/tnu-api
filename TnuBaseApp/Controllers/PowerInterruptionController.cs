using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace TnuBaseApp.Controllers
{
    public class PowerInterruptionController : ApiController
    {
        //Returns Json
        public IQueryable<Location> Get(string id)
        {
            var piScraper = new WpPiScraper(id);
            return piScraper.GetInteruptionData().OrderByDescending(l => l.IsInterrupted);
           
        }

    }

    public class WpPiScraper
    {
        public readonly string _url = "http://www.westernpower.com.au/online/pii/?location={0}";
        private bool IsPostCodeSearch;

        public WpPiScraper(string location)
        {
            _url = string.Format(_url, location);
            int postCode = 0;
            IsPostCodeSearch = int.TryParse(location, out postCode);
        }

        public IQueryable<Location> GetInteruptionData()
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
                    var locationString = resultLine[0].InnerHtml;
                    var location = new Location()
                    {
                        Name = locationString.Substring(0, locationString.IndexOf("(") - 1),
                        PostCode = locationString.Substring(locationString.IndexOf("(") + 1, 4),
                        Details = resultLine[1].InnerHtml,
                    };
                    locationList.Add(location);
                }
            }

            return locationList.AsQueryable();
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
        public string PostCode { get; set; }
        public string Details { get; set; }

        #region Derived Properties

        public string Summary
        {
            get
            {
                return Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()) ? "No Interruptions" : "Service Down";

            }
        }
        public string RestorationTime
        {
            get
            {
                return Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()) ? "N/A": GetRestorationTime(Details);
            }
        }
        public bool IsInterrupted { get { return !Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()); } }

        #endregion


        private string GetRestorationTime(string details)
        {
            var restoration = details.Substring(details.IndexOf(':') + 1).Trim(); //Expected Restoration : 20/05/2013 13:00 
            DateTime restoreTime;
            if (DateTime.TryParse(restoration, out restoreTime))
            {
                return restoreTime.ToString("dd/MM/yyyy HH:mm");
            }
            return restoration;
        }
    }

}