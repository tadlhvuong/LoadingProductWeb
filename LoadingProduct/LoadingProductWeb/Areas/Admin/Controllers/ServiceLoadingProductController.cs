using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadingProductShared.Data;
using LoadingProductShared.Helpers;
using LoadingProductWeb.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Update(int id)
        {
            var model = _dbContext.Services.Find(id);
            if (model == null)
                return BadRequest();

           return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(Service model)
        {
            if (ModelState.IsValid)
            {
                model.LastUpdate = DateTime.Now;
                model.UpdateUser = User.Identity.Name;
                _dbContext.Entry(model).State = EntityState.Modified;
                _dbContext.SaveChanges();
                
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: BlogPost/Delete/5
        public IActionResult Delete(int? id)
        {
            var model = _dbContext.Services.Find(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(Service model)
        {
            try
            {
                _dbContext.Entry(model).State = EntityState.Deleted;
                _dbContext.SaveChanges();
                return Json(new ModalFormResult() { Code = 1 });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

    }
}