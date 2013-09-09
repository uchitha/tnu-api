using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using TnuBaseApp.Controllers;

namespace TnuBaseApp
{
    public class PostCodeFetcher
    {
        /// <summary>
        /// Returns a JSON object of all PostCodes in WA
        /// </summary>
        /// <returns></returns>
        public JArray GetPostCodes(int start, int end)
        {
            var container = new JArray();
            for (int i = start; i < end; i++)
            {
                var postCode = i.ToString(CultureInfo.InvariantCulture);
                var suburbList = GetPostCodesFromAusPost(postCode);
                if (suburbList.Equals("[]")) continue;
                var jsonObject = ComposePostCodeJson(postCode, suburbList);
                container.Add(jsonObject);
            }
            return container;
        }

        /// <summary>
        /// Updates the post code master data file
        /// </summary>
        /// <param name="postCodeInfo"></param>
        public int UpdatePostCodeInfo(JArray postCodeInfo,string filePath)
        {
            var data = JsonConvert.SerializeObject(postCodeInfo, Formatting.Indented, new JsonSerializerSettings());
            System.IO.File.WriteAllText(filePath, data);
            return postCodeInfo.Count;
        }

        public DateTime FetchLastUpdatedTimeStamp(string postCodeFilePath)
        {
            var postCodeUpdateTime = new FileInfo(postCodeFilePath).LastWriteTime;
            return postCodeUpdateTime.AddHours(8); //Convert to GMT+8
        }


        private JObject ComposePostCodeJson(string postCode, string suburbString)
        {
            var parentJson = new JObject(new JProperty("PostCode", postCode));
            parentJson.Add("Suburbs", suburbString);
            return parentJson;
        }


        private string GetPostCodesFromAusPost(string postCode)
        {
            var ausPostApi = new AusPostController();
            var suburbList = ausPostApi.Get(postCode);
            return JsonConvert.SerializeObject(suburbList);
        }
    }
}