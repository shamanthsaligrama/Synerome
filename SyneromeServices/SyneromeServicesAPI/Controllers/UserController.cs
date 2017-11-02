using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyneromeServicesAPI.Models;

namespace SyneromeServicesAPI.Controllers
{
    [Route("api/User")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddUsers([FromBody]Users userObj)
        {
            if (userObj == null)
                return BadRequest();

            return Ok();
        }
    }
}