using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

        public async Task<ActionResult> LoadPostCodeDataAsync()
        {
            var fetcher = new PostCodeFetcher();
            var start = int.Parse(ConfigurationManager.AppSettings["startPostCode"]);
            var end = int.Parse(ConfigurationManager.AppSettings["endPostCode"]);

            var postCodeDataAsJson = fetcher.GetPostCodes(start,end); //WA Only
            var postCodeFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.PostCodeFile);
            var count = fetcher.UpdatePostCodeInfo(postCodeDataAsJson,postCodeFilePath);
            ViewBag.Message = string.Format("Updated {0} locations", count);
            return View("Index");
        }

        public ActionResult LoadAllInterruptionInfo()
        {
            var fetcher = new PowerInterruptionFetcher();
            var postCodeFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.PostCodeFile);
            var interruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.InterruptionInfoFile);

            var interruptionDataAsJson = fetcher.FetchInterruptions(postCodeFilePath);
            var count = fetcher.UpdateInterruptionInfo(interruptionDataAsJson, interruptionFilePath);
            ViewBag.Message = string.Format("Updated {0} interruptions", count);
            return View("Index");
        }

        public ActionResult LoadCurrentInterruptionInfo()
        {
            var fetcher = new PowerInterruptionFetcher();
            var interruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.InterruptionInfoFile);
            var currentInterruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.CurrentInterruptionInfoFile);

            var currentInterruptionDataAsJson = fetcher.FetchCurrentIntteruptions(interruptionFilePath);

            var count = fetcher.UpdateCurrentInterruptionInfo(currentInterruptionDataAsJson, currentInterruptionFilePath);
            ViewBag.Message = string.Format("Updated {0} interruptions", count);
            return View("Index");
        }

       
    }
}
