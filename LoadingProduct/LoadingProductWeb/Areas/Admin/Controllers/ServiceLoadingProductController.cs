using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadingProductShared.Data;
using LoadingProductShared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoadingProductWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServiceLoadingProductController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;

        public ServiceLoadingProductController(
            ILogger<ServiceLoadingProductController> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(PagedList<Service> model)
        {
            var filterQuery = _dbContext.Services.Where(x => model.Search == null || x.Title.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            return View(model);
        }
        public ActionResult Details(int id)
        {
            var model = _dbContext.Services.Find(id);
            if (model == null)
                return BadRequest();

            return View(model);
        }

        public ActionResult Create()
        {
            return View(new Service());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Service model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                model.CreateUser = User.Identity.Name;
                model.UpdateUser = User.Identity.Name;
                model.CreateTime = DateTime.Now;
                model.PublishTime = DateTime.Now;
                _dbContext.Services.Add(model);

                _dbContext.SaveChanges();
                model = new Service();
                return Redirect(Url.Action("Index"));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
    }
}