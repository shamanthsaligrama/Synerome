using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SyneromeServicesAPI.Controllers
{
    public class NutritionistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}