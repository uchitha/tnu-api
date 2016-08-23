using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TnuBaseApp.Models
{  
    public class ContactInfo
    {
        public int NumberOfItems { get; set; }
        public List<SelectListItem> ItemQuantityList { get; set; }
        public string KidsName { get; set; }
        public string KidsAge { get; set; }
        public string AdultName { get; set; }
        public string AdultEmail { get; set; }
        public string Comments { get; set; }
        public string Programme { get; set; }
      
    }
}
