using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TnuBaseApp.Models
{  
    public class Order
    {
        public int NumberOfItems { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }
    }
}
