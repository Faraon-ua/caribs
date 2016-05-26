using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Caribs.Common.Helpers;

namespace Caribs.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return View();
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

        public ActionResult Tourist()
        {
            return View();
        }

        public ActionResult TourOperator()
        {
            return View();
        }
        public ActionResult University()
        {
            return View();
        }
        public ActionResult Business()
        {
            return View();
        }
        public ActionResult Investor()
        {
            return View();
        }
        public ActionResult Seller()
        {
            return View();
        }
    }
}