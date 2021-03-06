﻿using Newtonsoft.Json;
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
using NLog;
using TnuBaseApp.Models;

namespace TnuBaseApp.Controllers
{
    public class AdminController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Info("COMPLETED: Postcode information finished loading from AusPost");

            });
            logger.Info("START: Postcode information started loading from AusPost");
            ViewBag.Message = "Postcode update triggered (Will take around 20 mins to complete)";
            return View("Index");
        }

        public ActionResult LoadAllInterruptionInfo()
        {
            var fetcher = new PowerInterruptionFetcher();
            var postCodeFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.PostCodeFile);
            var interruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.InterruptionInfoFile);
            var currentInterruptionsInfoFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.CurrentInterruptionInfoFile);

            var lastUpdateTime = fetcher.FetchLastUpdatedTimeStamp(interruptionFilePath);

            if (lastUpdateTime.AddMinutes(10) > DateTime.Now)
            {
                logger.Info("Interruption update not triggered because it's been triggered during last 10 minutes");
                ViewBag.Message = "Interruption update not triggered because it's been triggered during last 10 minutes";
                return View("Index");
            }

            ThreadPool.QueueUserWorkItem(s =>
            {
                var interruptionDataAsJson = fetcher.FetchInterruptions(postCodeFilePath).Result;
                fetcher.UpdateInterruptionInfo(interruptionDataAsJson, interruptionFilePath,currentInterruptionsInfoFilePath);
                logger.Info("COMPLETED: Interruption information finished loading from Western Power");
            });
            logger.Info("START: Interruption information started loading from Western Power");
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

        public string Demo()
        {
            var pf = new PowerInterruptionFetcher();
            var interruptionFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.InterruptionInfoFile);
            var currentInterruptionsInfoFilePath = HttpContext.Server.MapPath("~/App_Data/" + AppConstants.CurrentInterruptionInfoFile);

            var currInterruptions = pf.FetchCurrentIntteruptions(interruptionFilePath);
            if (currInterruptions.Count > 1) return "No need to demo";

            pf.CreateCurrentInterruptionsForDemo(currentInterruptionsInfoFilePath,interruptionFilePath);

            return "Demo mode initiated";
        }

        private Dictionary<string, string> GetLastModifiedTimes(string postCodeFile, string interruptionFile)
        {
            var details = new Dictionary<string, string>();
            details.Add("postcode", new PostCodeFetcher().FetchLastUpdatedTimeStamp(postCodeFile).ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz"));
            details.Add("interruptions",  new PowerInterruptionFetcher().FetchLastUpdatedTimeStamp(interruptionFile).ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz"));

            return details;

        }
       
    }
}
