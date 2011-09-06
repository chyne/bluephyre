using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Logging;

namespace BluePhyre.Controllers
{
    public class HomeController : Controller
    {

        public ILogger Logger { get; set; }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            Logger.Debug("Test!");
            return View();
        }

    }
}
