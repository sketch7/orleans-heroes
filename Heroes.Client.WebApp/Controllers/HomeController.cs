using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Heroes_Client_WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
	        ViewData["bootstrapConfig"] = new
	        {
				Username = "horhay",
				IsDebug = true
			};

			return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
