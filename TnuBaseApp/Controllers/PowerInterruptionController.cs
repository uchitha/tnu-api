using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using TnuBaseApp.Models;

namespace TnuBaseApp.Controllers
{
    public class PowerInterruptionController : ApiController
    {

        private const string InterruptionInfoFile = @"~\App_Data\westernpower_interruptions.txt";
        private const string CurrentInterruptionInfoFile = @"~\App_Data\westernpower_current_interruptions.txt";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<InterruptionInfo> Get(string id)
        {

            if (id.ToLowerInvariant().Equals("servicedown"))
            {
                var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(CurrentInterruptionInfoFile));
                var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent);
                return list;
            }

            if (id.ToLowerInvariant().Equals("all"))
            {
                var interruptionContent = File.ReadAllText(HttpContext.Current.Server.MapPath(InterruptionInfoFile));
                var list = JsonConvert.DeserializeObject<List<InterruptionInfo>>(interruptionContent);
                return list;
            }

            if (!IsDataStale())
            {
                logger.Info("Using existing data as it's not stale");
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
            logger.Info("Calling western power to get latest data");
            return GetInterruptions(id);
           
        }

        [HttpGet]
        [ActionName("LastUpdatedTimeStamp")]
        public DateTime LastUpdatedTimeStamp()
        {
            var f = new PowerInterruptionFetcher();
            return f.FetchLastUpdatedTimeStamp(HttpContext.Current.Server.MapPath(CurrentInterruptionInfoFile));
        }

        [HttpGet]
        [ActionName("RealTime")]
        public IEnumerable<InterruptionInfo> RealTime(string id)
        {
            return GetInterruptions(id);
        }

        private IEnumerable<InterruptionInfo> GetInterruptions(string location)
        {
            var pf = new PowerInterruptionFetcher();
            var result = pf.GetInterruptionFromWp(location).Result;
            logger.Info("COMPLETED: Interruption information finished loading from Western Power");
            return result;
        
        }

        private bool IsDataStale()
        {
            var pf = new PowerInterruptionFetcher();
            var lastUpdateTime = pf.FetchLastUpdatedTimeStamp(HttpContext.Current.Server.MapPath(CurrentInterruptionInfoFile));
            int updateWindow = 10;
            int.TryParse(ConfigurationManager.AppSettings["InterruptionUpdateWindowInMinutes"], out updateWindow);
            return (lastUpdateTime.AddMinutes(updateWindow) < DateTime.Now);
        }

    }






}