using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DataProviderToClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataProviderToClient.Controllers
{
    public class PowerInterruptionController : ApiController
    {
        private const string InterruptionInfoFile = @"~\App_Data\westernpower_interruptions.txt";
        private const string CurrentInterruptionInfoFile = @"~\App_Data\westernpower_current_interruptions.txt";

        public IEnumerable<InterruptionInfo> Get()
        {
            var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
            var list =  JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent);

            return list;
        }

        public InterruptionInfo Get(string suburb)
        {
            var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
            var totalJson = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent);
            var interruption = totalJson.FirstOrDefault(i => i.Name.ToUpperInvariant().Equals(suburb.ToUpperInvariant()));
            return interruption;
        }

        public List<InterruptionInfo> Get(int postcode)
        {
            var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
            var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent).FindAll(i=>i.PostCode.Equals(postcode.ToString()));
            return list;
        }
    }
}
