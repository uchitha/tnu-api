using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TnuBaseApp.Controllers;
using TnuBaseApp.Models;

namespace TnuBaseApp
{
    public class PowerInterruptionFetcher
    {


        public async Task<JArray> FetchInterruptions(string postCodeFilePath)
        {

            var postCodeData = File.ReadAllText(postCodeFilePath);
            var locations = (JArray)JsonConvert.DeserializeObject(postCodeData);
            JArray interruptionList = new JArray();

            foreach (var instance in locations)
            {
                var location = (JObject)instance;
                var postCode = location.GetValue("PostCode").ToString();
                var scraper = new WpPiScraper(postCode);

                var interruptionsInfoForPostCode = await scraper.GetInteruptionData();

                if (interruptionsInfoForPostCode.Any())
                {
                    var interruptionInfo = interruptionsInfoForPostCode.ToList();
                    foreach (var interruption in interruptionInfo)
                    {
                        var asJson = JsonConvert.SerializeObject(interruption);
                        interruptionList.Add(JObject.Parse(asJson));
                    }
                   
                }
            }

            return interruptionList;
        }

        public async Task<IEnumerable<InterruptionInfo>> FetchInterruptions(List<string> locations)
        {
            var interruptionList = new List<InterruptionInfo>();

            foreach (var location in locations)
            {
                var scraper = new WpPiScraper(location);

                var interruptionsInfoForlocation = await scraper.GetInteruptionData().ConfigureAwait(false);

                if (interruptionsInfoForlocation.Any())
                {
                    var interruptionInfo = interruptionsInfoForlocation.ToList();
                    foreach (var interruption in interruptionInfo)
                    {
                        interruptionList.Add(interruption);
                    }
                }
            }

            return interruptionList;
        }

        public async Task<IEnumerable<InterruptionInfo>> GetInterruptionFromWp(string location)
        {

            var scraper = new WpPiScraper(location);

            try
            {
                var interruptionsInfoFromWp = await scraper.GetInteruptionData().ConfigureAwait(false);
                if (interruptionsInfoFromWp.Any())
                {
                    return interruptionsInfoFromWp.ToList();
                }
                return new List<InterruptionInfo>();
            }
            catch (Exception)
            {
                throw;
            }

        }


        public List<InterruptionInfo> FetchCurrentIntteruptions(string currentInterruptionsInfoFilePath)
        {
            var interruptionInfo = File.ReadAllText(currentInterruptionsInfoFilePath);
            var locationsJson = (JArray)JsonConvert.DeserializeObject(interruptionInfo);
            var list = new List<InterruptionInfo>();

            foreach (var token in locationsJson)
            {
                var instance = new InterruptionInfo();
                instance.Name = token.Value<string>("Name");
                instance.PostCode = token.Value<string>("PostCode");
                instance.Details = token.Value<string>("Details");
                list.Add(instance);
            }

            return list;
        } 

        public int UpdateInterruptionInfo(JArray info, string interruptionInfoFilePath,string currentInterruptionsInfoFilePath)
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
            
            UpdateCurrentInterruptionInfo(interruptionInfoFilePath,currentInterruptionsInfoFilePath);

            return info.Count();

        }

   
        public DateTime FetchLastUpdatedTimeStamp(string interruptionFilePath)
        {
            var interruptionUpdateTime = new FileInfo(interruptionFilePath).LastWriteTime; 
            return interruptionUpdateTime; 
        }

  

        private int UpdateCurrentInterruptionInfo(string interruptionInfoFilePath,string currentInterruptionsInfoFilePath)
        {
            //var interruptionsAsJson = JsonConvert.SerializeObject(info, Formatting.Indented);

            var interruptionInfo = File.ReadAllText(interruptionInfoFilePath);
            var locations = (JArray)JsonConvert.DeserializeObject(interruptionInfo);

            var info = locations.Where(l => l.Value<bool>("IsInterrupted")).OrderBy(i=>i.Value<string>("Name")).ToList();

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

        public int CreateCurrentInterruptionsForDemo(string currentInterruptionsInfoFilePath, string interruptionInfoFilePath)
        {
            var interruptionInfo = File.ReadAllText(interruptionInfoFilePath);
            var locations = (List<InterruptionInfo>)JsonConvert.DeserializeObject(interruptionInfo,typeof(List<InterruptionInfo>));

            var choosenOnes = locations.Where(l => l.Name.StartsWith("A"));
            int count = 0;
            foreach (var item in choosenOnes)
            {
                try
                {
                    var restorationTime = DateTime.Now.AddHours(4);
                    item.Details = "Estimated Restoration: " + restorationTime.ToString("dd/MM/yyyy HH:mm:ss");
                    count++;
                    if (count >= 5) return count;
                }
                catch (Exception)
                {

                }
            }
            return count;
        }


      
    }
}