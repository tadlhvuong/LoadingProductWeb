using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadingProductShared.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoadingProductWeb.Areas.Admin
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        #region General

        private readonly AppDBContext _dbContext;
        private readonly ILogger<HomeAdminController> _logger;

        public HomeAdminController(AppDBContext dbContext,
            ILogger<HomeAdminController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        #endregion
    }
}