using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TnuBaseApp.Controllers;

namespace TnuBaseApp
{
    public class PowerInterruptionFetcher
    {
        public JArray FetchInterruptions(string postCodeFilePath)
        {

            var postCodeData = File.ReadAllText(postCodeFilePath);
            var locations = (JArray)JsonConvert.DeserializeObject(postCodeData);
            JArray interruptionList = new JArray();

            foreach (var instance in locations)
            {
                var location = (JObject)instance;
                var postCode = location.GetValue("PostCode").ToString();
                var scraper = new WpPiScraper(postCode);
                var interruptionsInfoForPostCode = scraper.GetInteruptionData();
                if (interruptionsInfoForPostCode.Any())
                {
                    var interruptionInfo = scraper.GetInteruptionData().ToList();
                    foreach (var interruption in interruptionInfo)
                    {
                        var asJson = JsonConvert.SerializeObject(interruption);
                        interruptionList.Add(JObject.Parse(asJson));
                    }
                   
                }
            }

            return interruptionList;
        }

        public List<JToken> FetchCurrentIntteruptions(string interruptionInfoFilePath)
        {
            var interruptionInfo = File.ReadAllText(interruptionInfoFilePath);
            var locations = (JArray)JsonConvert.DeserializeObject(interruptionInfo);

            return locations.Where(l => l.Value<bool>("IsInterrupted")).ToList();
        } 

        public int UpdateInterruptionInfo(JArray info, string interruptionInfoFilePath)
        {
            var parentJson = new JArray();
            foreach (var interruption in info) //Converting one by one so that if any one location fails, others are not affected
            {
                var location = interruption.Value<string>("Name");
                try
                {
                    var interruptionJsonString = JsonConvert.SerializeObject(interruption);
                    var interruptionJsonObj = JObject.Parse(interruptionJsonString);
                    parentJson.Add(interruptionJsonObj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error converting location {0}", location);
                    Console.WriteLine(ex.Message);
                }

            }
            var fullIntteruptionInfo = JsonConvert.SerializeObject(parentJson, Formatting.Indented);
            File.WriteAllText(interruptionInfoFilePath, fullIntteruptionInfo);
            return info.Count();

        }

        public int UpdateCurrentInterruptionInfo(IEnumerable<JToken> info,string currentInterruptionsInfoFilePath)
        {
            //var interruptionsAsJson = JsonConvert.SerializeObject(info, Formatting.Indented);

            var parentJson = new JArray();
            foreach (var location in info)
            {
                try
                {
                    parentJson.Add(location);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error converting location {0}", location.Value<string>("Name"));
                    Console.WriteLine(ex.Message);
                }

            }
            var currentIntteruptionInfo = JsonConvert.SerializeObject(parentJson, Formatting.Indented);
            File.WriteAllText(currentInterruptionsInfoFilePath, currentIntteruptionInfo);
            return info.Count();
        }

        
    }
}