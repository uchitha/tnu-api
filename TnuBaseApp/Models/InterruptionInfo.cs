using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TnuBaseApp.Models
{
    public class InterruptionInfo
    {
        private readonly string NoInterruptionText = "No known interruptions";

        public string Name { get; set; }
        public string PostCode { get; set; }
        public string Details { get; set; }

        #region Derived Properties

        public string Summary
        {
            get
            {
                return Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()) ? "No Interruptions" : "Service Down";

            }
        }
        public string RestorationTime
        {
            get
            {
                return Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()) ? "N/A" : GetRestorationTime(Details);
            }
        }
        public bool IsInterrupted { get { return !Details.ToUpperInvariant().Equals(NoInterruptionText.ToUpperInvariant()); } }

        #endregion


        private string GetRestorationTime(string details)
        {
            var restoration = details.Substring(details.IndexOf(':') + 1).Trim(); //Expected Restoration : 20/05/2013 13:00 
            DateTime restoreTime;
            var culture = CultureInfo.CreateSpecificCulture("en-AU");
            var styles = DateTimeStyles.None;
            if (DateTime.TryParse(restoration, culture, styles, out restoreTime))
            {
                return restoreTime.ToString("dd/MM/yyyy HH:mm");
            }
            return restoration;
        }
    }
}
