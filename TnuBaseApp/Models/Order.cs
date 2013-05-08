using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TnuBaseApp.Models
{
    public class Order
    {
        public Order()
        {
            ItemQuantityList = CreateList(10);
        }

        public int NumberOfItems { get; set; }
        public List<SelectListItem> ItemQuantityList { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }

        public string OrderStatus { get; set; }

        private List<SelectListItem> CreateList(int max)
        {
            var list = new List<SelectListItem>();
            for (int i = 1; i <= max; i++)
            {
                var item = new SelectListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                list.Add(item);
            }

            return list;
        }
    }
}