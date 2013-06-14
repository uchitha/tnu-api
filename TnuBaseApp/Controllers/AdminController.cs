using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TnuBaseApp.Controllers
{
    public class AdminController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadPostCodeData()
        {
            var fetcher = new PostCodeFetcher();
            var start = int.Parse(ConfigurationManager.AppSettings["startPostCode"]);
            var end = int.Parse(ConfigurationManager.AppSettings["endPostCode"]);

            ThreadPool.QueueUserWorkItem(s =>
            {
                var postCodeDataAsJson = fetcher.GetPostCodes(start, end); //WA Only
                var postCodeFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.PostCodeFile);
                var count = fetcher.UpdatePostCodeInfo(postCodeDataAsJson, postCodeFilePath);
            });

            ViewBag.Message = "Postcode update triggered (Will take around 20 mins to complete)";
            return View("Index");
        }

        public ActionResult LoadAllInterruptionInfo()
        {
            var fetcher = new PowerInterruptionFetcher();
            var postCodeFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.PostCodeFile);
            var interruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.InterruptionInfoFile);
            var currentInterruptionsInfoFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.CurrentInterruptionInfoFile);

            ThreadPool.QueueUserWorkItem(s =>
            {
                var interruptionDataAsJson = fetcher.FetchInterruptions(postCodeFilePath).Result;
                fetcher.UpdateInterruptionInfo(interruptionDataAsJson, interruptionFilePath,currentInterruptionsInfoFilePath);
            });

            ViewBag.Message = "Interruption update triggered";
            return View("Index");
        }

        public ActionResult GetDataTimeStamp()
        {
            var postCodeFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.PostCodeFile);
            var interruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.InterruptionInfoFile);

            var details = GetLastModifiedTimes(postCodeFilePath, interruptionFilePath);
            ViewBag.Message = string.Format("Post Codes : Last updated on {0}, Interruptions : Last updated on {1}", details["postcode"], details["interruptions"]);
            return View("Index");
        }

        private Dictionary<string, DateTime> GetLastModifiedTimes(string postCodeFile, string interruptionFile)
        {
            var postCodeUpdateTime = new FileInfo(postCodeFile).LastWriteTime;
            var interruptionUpdateTime = new FileInfo(interruptionFile).LastWriteTime;

            var details = new Dictionary<string, DateTime>();
            details.Add("postcode", postCodeUpdateTime);
            details.Add("interruptions", interruptionUpdateTime);

            return details;

        }
       
    }
}
