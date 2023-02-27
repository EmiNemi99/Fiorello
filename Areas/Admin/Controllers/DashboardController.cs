using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Areas.Admin.Controllers
{
     [Area(nameof(Admin))]
   
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
