using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using TCVWeb.Areas.Admin.Models;

namespace TCVWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Manager,Operator")]
    public class BlogPostController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;

        public BlogPostController(
            ILogger<BlogPostController> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(PagedList<BlogPost> model)
        {
            var filterQuery = _dbContext.BlogPosts.Where(x => model.Search == null || x.Title.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _dbContext.BlogPosts.Find(id);
            if (model == null)
                return BadRequest();

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.PostCats = _dbContext.PostCats.Select(x => new SelectItemModel() { id = x.Id, text = x.Name }).ToList();
            return View(new BlogPost());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(BlogPost model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                model.CreateUser = User.Identity.Name;
                model.UpdateUser = User.Identity.Name;
                model.CreateTime = DateTime.Now;
                if (model.Status == PostStatus.Normal || model.Status == PostStatus.Special)
                    model.PublishTime = DateTime.Now;
                if (model.Status == PostStatus.Pending || model.Status == PostStatus.Suspended)
                    model.PublishTime = null;
                _dbContext.BlogPosts.Add(model);

                if (model.PostCats != null)
                {
                    foreach (var catId in model.PostCats)
                    {
                        BlogPostTaxo newCat = new BlogPostTaxo()
                        {
                            TaxoId = catId,
                            PostId = model.Id,
                        };
                        _dbContext.BlogPostTaxoes.Add(newCat);
                    }
                }

                if (!string.IsNullOrEmpty(model.PostTags))
                {
                    string[] postTags = model.PostTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tag in postTags)
                    {
                        string trTag = tag.Trim();
                        Taxonomy taxo = _dbContext.PostTags.FirstOrDefault(x => x.Name == trTag);
                        if (taxo == null)
                        {
                            taxo = new Taxonomy() { Type = TaxoType.PostTag, Name = trTag };
                            _dbContext.Taxonomies.Add(taxo);
                            _dbContext.SaveChanges();
                        }

                        BlogPostTaxo newTag = new BlogPostTaxo()
                        {
                            TaxoId = taxo.Id,
                            PostId = model.Id,
                        };
                        _dbContext.BlogPostTaxoes.Add(newTag);
                    }
                }
                _dbContext.SaveChanges();
                model = new BlogPost();
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
            var model = _dbContext.BlogPosts.Find(id);
            if (model == null)
                return BadRequest();

            model.PostCats = (from x in _dbContext.Taxonomies
                              join y in _dbContext.BlogPostTaxoes on x.Id equals y.TaxoId
                              where y.PostId == id && x.Type == TaxoType.PostCat
                              select x.Id).ToArray();

            var postTags = (from x in _dbContext.Taxonomies
                            join y in _dbContext.BlogPostTaxoes on x.Id equals y.TaxoId
                            where y.PostId == id && x.Type == TaxoType.PostTag
                            select x.Name).ToArray();

            model.PostTags = string.Join(", ", postTags);

            model.LastStatus = model.Status;

            ViewBag.PostCats = _dbContext.PostCats.Select(x => new SelectItemModel() { id = x.Id, text = x.Name, selected = model.PostCats.Contains(x.Id) }).ToList();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(BlogPost model)
        {
            if (ModelState.IsValid)
            {
                model.LastUpdate = DateTime.Now;
                model.UpdateUser = User.Identity.Name;
                if (model.Status == PostStatus.Normal || model.Status == PostStatus.Special)
                    model.PublishTime = DateTime.Now;
                if (model.Status == PostStatus.Pending || model.Status == PostStatus.Suspended)
                    model.PublishTime = null;

                _dbContext.Entry(model).State = EntityState.Modified;
                _dbContext.SaveChanges();

                // Remove all taxoes
                var delItems = (from item in _dbContext.BlogPostTaxoes
                                where item.PostId == model.Id
                                select item);

                foreach (var item in delItems)
                    _dbContext.BlogPostTaxoes.Remove(item);

                // Add cats
                if (model.PostCats != null)
                {
                    foreach (var catId in model.PostCats)
                    {
                        BlogPostTaxo newCat = new BlogPostTaxo()
                        {
                            TaxoId = catId,
                            PostId = model.Id,
                        };
                        _dbContext.BlogPostTaxoes.Add(newCat);
                    }
                }

                // Add tags
                if (!string.IsNullOrEmpty(model.PostTags))
                {
                    string[] postTags = model.PostTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tag in postTags)
                    {
                        string trTag = tag.Trim();
                        Taxonomy taxo = _dbContext.PostTags.FirstOrDefault(x => x.Name == trTag);
                        if (taxo == null)
                        {
                            taxo = new Taxonomy() { Type = TaxoType.PostTag, Name = trTag };
                            _dbContext.Taxonomies.Add(taxo);
                            _dbContext.SaveChanges();
                        }

                        BlogPostTaxo newTag = new BlogPostTaxo()
                        {
                            TaxoId = taxo.Id,
                            PostId = model.Id,
                        };

                        _dbContext.BlogPostTaxoes.Add(newTag);
                    }
                }

                _dbContext.SaveChanges();
                return RedirectToAction("Details", new { @id = model.Id });
            }

            ViewBag.PostCats = _dbContext.PostCats.Select(x => new SelectItemModel() { id = x.Id, text = x.Name, selected = model.PostCats.Contains(x.Id) }).ToList();
            return View(model);
        }

        // GET: BlogPost/Delete/5
        public IActionResult Delete(int? id)
        {
            var model = _dbContext.BlogPosts.Find(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(BlogPost model)
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

        public ActionResult PostCatList(PagedList<Taxonomy> model)
        {
            var filterQuery = _dbContext.PostCats.Where(x => model.Search == null || x.Name.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            return View(model);
        }

        public ActionResult PostCatCreate()
        {
            ViewBag.ParentId = new SelectList(_dbContext.PostCats, "Id", "Name");
            return View("PostCatEdit", new Taxonomy());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PostCatCreate(Taxonomy model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Type = TaxoType.PostCat;
                    _dbContext.Taxonomies.Add(model);
                    _dbContext.SaveChanges();
                    return Json(new ModalFormResult() { Code = 1 });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.ParentId = new SelectList(_dbContext.PostCats, "Id", "Name");
            return View("PostCatEdit", model);
        }

        public ActionResult PostCatUpdate(int? id)
        {
            Taxonomy model = _dbContext.Taxonomies.Find(id);
            if (model == null)
                return BadRequest();

            ViewBag.ParentId = new SelectList(_dbContext.PostCats, "Id", "Name");
            return View("PostCatEdit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PostCatUpdate(Taxonomy model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Entry(model).State = EntityState.Modified;
                    model.Type = TaxoType.PostCat;
                    _dbContext.SaveChanges();
                    return Json(new ModalFormResult() { Code = 1 });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.ParentId = new SelectList(_dbContext.PostCats, "Id", "Name");
            return View("PostCatEdit", model);
        }
        public IActionResult PostCatDelete(int? id)
        {
            var model = _dbContext.Taxonomies.Find(id);
            if (model == null)
                return NotFound();

            return View("PostCatDelete", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult PostCatDelete(Taxonomy model)
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

            return View("PostCatDelete", model);
        }

    }
}