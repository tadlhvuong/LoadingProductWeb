using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCVShared.Data;
using TCVShared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TCVWeb.Models;
using TCVWeb.Areas.Admin.Models;

namespace TCVWeb.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<BlogPostController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public BlogPostController(AppDBContext dbContext,
            ILogger<BlogPostController> logger,
            UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }
        // GET: BlogPost
        public async Task<IActionResult> Index(PagedList<BlogPost> model, int pageSize)
        {
            var filterQuery = _dbContext.BlogPosts.Where(x => model.Search == null || x.Title.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = await selectQuery.ToListAsync();
            model.PageSize = pageSize != 0 ? pageSize : 12;

            return View(model);
        }

         //GET: BlogPost/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.PostCats = _dbContext.PostCats.Select(x => new SelectItemModel() { id = x.Id, text = x.Name }).ToList();
            var blogModel = _dbContext.BlogPosts.FirstOrDefault(blog => blog.Id == id);
            return View(blogModel);
        }

        private List<Taxonomy> GetTagsItem(int? id)
        {
            var idTaxos = _dbContext.BlogPostTaxoes.Where(x => x.PostId == id).Select(x => x.TaxoId).ToList();
            List<Taxonomy> taxoTags = _dbContext.Taxonomies.Where(x => idTaxos.Contains(x.Id) && x.Type == TaxoType.PostTag).ToList();
            foreach (var item in idTaxos)
            {
                var taxo = _dbContext.Taxonomies.FirstOrDefault(x => x.Id == item && x.Type == TaxoType.PostTag);
                if (taxo != null && taxo.ParentId != null)
                    if (!idTaxos.Contains(taxo.Id))
                        taxoTags.Add(taxo);
                    else
                    {
                        if (!idTaxos.Contains(taxo.Id))
                            taxoTags.Add(taxo);
                        var parent = _dbContext.Taxonomies.Find(taxo.ParentId);
                        if (!idTaxos.Contains(parent.Id))
                            taxoTags.Add(parent);
                    }
            }
            return taxoTags;
        }

        //// GET: BlogPost/Create
        //[HttpPost]
        //public ActionResult Create(BlogPost model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);
        //    try
        //    {
        //        model.CreateUser = User.Identity.Name;
        //        model.UpdateUser = User.Identity.Name;
        //        model.CreateTime = DateTime.Now;
        //        if (model.Status == PostStatus.Normal || model.Status == PostStatus.Special)
        //            model.PublishTime = DateTime.Now;
        //        _dbContext.BlogPosts.Add(model);

        //        if (model.PostCats != null)
        //        {
        //            foreach (var catId in model.PostCats)
        //            {
        //                BlogPostTaxo newCat = new BlogPostTaxo()
        //                {
        //                    TaxoId = catId,
        //                    PostId = model.Id,
        //                };
        //                _dbContext.BlogPostTaxoes.Add(newCat);
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(model.PostTags))
        //        {
        //            string[] postTags = model.PostTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            foreach (var tag in postTags)
        //            {
        //                string trTag = tag.Trim();
        //                Taxonomy taxo = _dbContext.PostTags.FirstOrDefault(x => x.Name == trTag);
        //                if (taxo == null)
        //                {
        //                    taxo = new Taxonomy() { Type = TaxoType.PostTag, Name = trTag };
        //                    _dbContext.Taxonomies.Add(taxo);
        //                    _dbContext.SaveChanges();
        //                }

        //                BlogPostTaxo newTag = new BlogPostTaxo()
        //                {
        //                    TaxoId = taxo.Id,
        //                    PostId = model.Id,
        //                };
        //                _dbContext.BlogPostTaxoes.Add(newTag);
        //            }
        //        }
        //        _dbContext.SaveChanges();
        //        model = new BlogPost();
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        return View();
        //    }
        //}


        //private List<Taxonomy> GetCategoriesItem(int? id)
        //{
        //    var idTaxos = _dbContext.BlogPostTaxoes.Where(x => x.PostId == id).Select(x => x.TaxoId).ToList();
        //    List<Taxonomy> taxoCates = _dbContext.Taxonomies.Where(x => idTaxos.Contains(x.Id) && x.Type == TaxoType.PostCat).ToList();
        //    foreach (var item in idTaxos)
        //    {
        //        var taxo = _dbContext.Taxonomies.FirstOrDefault(x => x.Id == item && x.Type == TaxoType.PostCat);
        //        if (taxo != null && taxo.ParentId != null)
        //            if (!idTaxos.Contains(taxo.Id))
        //                taxoCates.Add(taxo);
        //            else
        //            {
        //                if (!idTaxos.Contains(taxo.Id))
        //                    taxoCates.Add(taxo);
        //                var parent = _dbContext.Taxonomies.Find(taxo.ParentId);
        //                if (!idTaxos.Contains(parent.Id))
        //                    taxoCates.Add(parent);
        //            }
        //    }
        //    return taxoCates;
        //}

    }
}