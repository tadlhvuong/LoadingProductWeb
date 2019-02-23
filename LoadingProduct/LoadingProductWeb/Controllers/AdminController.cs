using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadingProductWeb.Areas.Admin;
using LoadingProductWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingProductWeb.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(AccountController.Login), "Account", new { area = "Admin" });
        }
    }
}