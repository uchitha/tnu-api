using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using TnuBaseApp.Models;

namespace TnuBaseApp.Controllers
{
    public class PowerInterruptionController : ApiController
    {

        private const string InterruptionInfoFile = @"~\App_Data\westernpower_interruptions.txt";
        private const string CurrentInterruptionInfoFile = @"~\App_Data\westernpower_current_interruptions.txt";

        public IEnumerable<InterruptionInfo> Get()
        {
                var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
                var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent);
                return list;
        }

        public IEnumerable<InterruptionInfo> Get(string id)
        {

            if (id.ToLowerInvariant().Equals("servicedown"))
            {
                var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(CurrentInterruptionInfoFile));
                var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent);
                return list;
            }

            int postcode;
            if (int.TryParse(id, out postcode))
            {
                var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
                var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent).FindAll(i => i.PostCode.Equals(postcode.ToString())).OrderByDescending(info => info.IsInterrupted);
                return list;
            }
            else
            {
                var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
                var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent).FindAll(i => i.Name.ToUpperInvariant().Equals(id.ToUpperInvariant())).OrderByDescending(info => info.IsInterrupted);
                return list; //Generally only 1 element
            }
           
        }

        [HttpGet]
        public DateTime GetLastUpdatedTimeStamp()
        {
            var f = new PowerInterruptionFetcher();
            return f.FetchLastUpdatedTimeStamp(HttpContext.Current.Server.MapPath(CurrentInterruptionInfoFile));
        }

    }

  


   

}