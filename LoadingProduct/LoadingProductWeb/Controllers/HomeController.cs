using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using TCVWeb.Models;

namespace TCVWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDBContext dbContext,
            ILogger<HomeController> logger,
            UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: FeatureProduct 
        public IActionResult Index(PagedList<ShopItem> model, int pageSize)
        {
            var a = HttpContext.Session?.GetString("language");
            if (a == null)
            {
                HttpContext.Session.SetString("language", "vi");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("vi");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi");
            }

            model.PageSize = pageSize != 0 ? pageSize : 18;

            var filterQuery = _dbContext.ShopItems.Where(x => model.Search == null);
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            foreach (var shopItem in model.Content)
            {
                shopItem.MediaFiles = _dbContext.MediaFiles.Where(file => file.AlbumId == shopItem.AlbumId).ToList();
            }

            var supplier = new Supplier();
            supplier.Name = "Ticivi Agriculture";
            supplier.Address = "/img/icon/logo.png";


            ViewData["brands"] = new Supplier[] { supplier, supplier, supplier, supplier, supplier, supplier, supplier };

            return View(model);
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult WarrantyPrivacy()
        {
            return View();
        }

        public IActionResult ShippingPrivacy()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult ChangeLanguage(string language, string returnURL)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            HttpContext.Session.SetString("language", language);


            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
