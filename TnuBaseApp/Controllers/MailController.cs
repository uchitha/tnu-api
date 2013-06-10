using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using TnuBaseApp.Models;

namespace TnuBaseApp.Controllers
{
    public class MailController : Controller
    {
        public ActionResult Index()
        {
            var mail = new MailObject();
            return View(mail);
        }

        public ActionResult Order()
        {
            var order = new Order();
            return View(order);
        }

        public ActionResult SendOrder(Order order)
        {
            var orderText = "<h3>You have a new order - {0}. </h3><br/><ul>Order Details <li>Number of Items : {1}</li><li>Name: {2}</li><li>Phone: {3}</li><li>Address: {4}</li></ul>";

            var mailObject = new MailObject()
            {
                From = "tnu@mailgun.org",
                Subject = "New Order",
                MessageText = "New order placed",
                MessageHtml = string.Format(orderText, DateTime.Now, order.NumberOfItems, order.CustomerName, order.CustomerPhoneNumber, order.CustomerAddress)
            };
            var response = SendSimpleMessage(mailObject);

            return RedirectToAction("Order");
        }

        public ActionResult Sent(MailObject mail)
        {
            return View("Index",mail);
        }

        
        public ActionResult Send(MailObject message)
        {
            var response = SendSimpleMessage(message);

            var sentMail = new MailObject()
            {
                SendStatus = response.StatusDescription
            };

            return RedirectToAction("Sent",sentMail);

        }


        private static IRestResponse SendSimpleMessage(MailObject message)
        {
            RestClient client = new RestClient();
            client.BaseUrl = "https://api.mailgun.net/v2";
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-7u19qg14wsvhjnlkp61l3k5y1051im31");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "app2956.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            
            request.AddParameter("to", "uchitha.r@gmail.com");
			request.AddParameter("to", "rexturf@bigpond.com");
            //request.AddParameter("to", "chris.ranasinghe@bhpbilliton.com");

            request.AddParameter("from", message.From);
            request.AddParameter("subject", message.Subject);
            request.AddParameter("text", message.MessageText);
            request.AddParameter("html", message.MessageHtml);
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }

    public class MailObject
    {
        public string MessageText { get; set; }
        public string MessageHtml { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        public string SendStatus { get; set; }
    }

    public class Order
    {
        public int NumberOfItems { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }
    }
}
