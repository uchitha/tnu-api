using System.Web.Http;
using NLog;
using TnuBaseApp.Models;

namespace TnuBaseApp.Controllers
{
    public class ContactUsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
       
        public void AussiKidsRegister(ContactInfo details)
        {
           logger.Info(details.KidsName);
        }

       
    }
}
