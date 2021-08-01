using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Vote_System.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {

                var user = User;

                bool isAdmin = User.IsInRole("Administrator");
                bool isVoter = User.IsInRole("Voter");

            }catch(Exception e)
            {
                Utils.LogException(e);
            }
            return View();
        }

        public ActionResult NavBar()
        {            
            return PartialView("_NavBar");
        }
        public ActionResult SideBarPartial()
        {            
            return PartialView("_SideBarPartial");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}