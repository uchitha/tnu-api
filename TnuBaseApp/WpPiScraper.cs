using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TnuBaseApp.Models;

namespace TnuBaseApp
{
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

        public async Task<IQueryable<InterruptionInfo>> GetInteruptionData()
        {
            var doc = new HtmlDocument();
            var html = await LoadPage();

            doc.LoadHtml(html);

            var details = doc.DocumentNode.SelectNodes("//table/tbody/tr");

            var locationList = new List<InterruptionInfo>();

            if (details != null && details.Count > 0)
            {
                foreach (var interuptionInfoItem in details)
                {
                    var resultLine = interuptionInfoItem.Descendants().Where(n => n.Name.Equals("td")).ToList();
                    var locationString = resultLine[0].InnerHtml;
                    var location = new InterruptionInfo()
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

        private async Task<string> LoadPage()
        {
            var content = new MemoryStream();
            var request = WebRequest.Create(_url);
            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    await responseStream.CopyToAsync(content);
                }
            }
            return System.Text.Encoding.Default.GetString(content.ToArray());
        }

    }
}