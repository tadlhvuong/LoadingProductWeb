using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using TCVWeb.Models;
using System.Linq;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCVWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<ProductController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDBContext dbContext,
    ILogger<ProductController> logger,
    UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;

        }

        //GET: Product/Detail/5
        public ActionResult Detail(int id)
        {
            ShopItem currentProduct = _dbContext.ShopItems.FirstOrDefault(product => product.Id == id);
            currentProduct.MediaFiles = _dbContext.MediaFiles.Where(file => file.AlbumId == currentProduct.AlbumId).ToList();

            ShopItem[] otherSizeProduct = _dbContext.ShopItems.Where(product => product.SKU.Substring(10, 3) == currentProduct.SKU.Substring(10, 3)).ToArray();

            var additionalInfomation = new List<AdditionInfo>();
            Taxonomy category = _dbContext.Taxonomies.FirstOrDefault(t => t.Id == int.Parse(currentProduct.SKU.Substring(0, 3)));
                 Taxonomy origin = _dbContext.Taxonomies.FirstOrDefault(t => t.Id == int.Parse(currentProduct.SKU.Substring(3, 1)));
               Taxonomy country = _dbContext.Taxonomies.FirstOrDefault(t => t.Id == int.Parse(currentProduct.SKU.Substring(4, 3)));
                Taxonomy size = _dbContext.Taxonomies.FirstOrDefault(t => t.Id == int.Parse(currentProduct.SKU.Substring(7, 2)));
             Taxonomy supplier = _dbContext.Taxonomies.FirstOrDefault(t => t.Id == int.Parse(currentProduct.SKU.Substring(9, 3)));

            additionalInfomation.Add(new AdditionInfo("Phân loại", category.Name));
            additionalInfomation.Add(new AdditionInfo("Xuất xứ", origin.Name));
            additionalInfomation.Add(new AdditionInfo("Nước sản xuất", country.Name));
            additionalInfomation.Add(new AdditionInfo("Khối lượng", size.Name));
            additionalInfomation.Add(new AdditionInfo("Nhà cung cấp ", "TICIVI"));

            ViewData["additionalInfomation"] = additionalInfomation;

            ViewData["currentProduct"] = currentProduct;

            ViewData["otherSizeProduct"] = otherSizeProduct;

            int productId = int.Parse(currentProduct.SKU.Substring(12, currentProduct.SKU.Length - 12));
            if (_dbContext.ShopItems.Any(o => int.Parse(o.SKU.Substring(12, o.SKU.Length - 12)) == productId + 1) == true) {
                ViewData["nextProduct"] = _dbContext.ShopItems.FirstOrDefault(o => int.Parse(o.SKU.Substring(12, o.SKU.Length - 12)) == productId + 1);
            }
            else {
                ViewData["nextProduct"] = currentProduct;
            }

            if (_dbContext.ShopItems.Any(o => int.Parse(o.SKU.Substring(12, o.SKU.Length - 12)) == productId - 1) == true){
                ViewData["previousProduct"] = _dbContext.ShopItems.FirstOrDefault(o => int.Parse(o.SKU.Substring(12, o.SKU.Length - 12)) == productId - 1);
            }
            else {
                ViewData["previousProduct"] = currentProduct;
            }

            
            ViewData["suggestedProduct"] = _dbContext.ShopItems.Where(product => product.Id < 5).ToArray();

            // Get comment;
            List<UserMessage> comments = _dbContext.UserMessages.Where(comment => comment.ThreadId == currentProduct.ThreadId).ToList();
            ViewData["comments"] = comments;

            return View();
        }

        public IActionResult Category(PagedList<ShopItem> model, int pageSize, int id, string from, string origin, int maxPrice, int minPrice, string style)
        {
            // Filter products  by category 
            var filterQuery = _dbContext.ShopItems.AsEnumerable();
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            if (id == -1){
                selectQuery = filterQuery.Where(x => x.SKU.Substring(0, 3) == "011" 
                                                    || x.SKU.Substring(0, 3) == "012"
                                                    || x.SKU.Substring(0, 3) == "013"
                                                    || x.SKU.Substring(0, 3) == "014").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            else if (id == -2) {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(0, 3) == "015"
                                                    || x.SKU.Substring(0, 3) == "016").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            else {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(0, 3) == id.ToString("D3")).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }

            // Filter products by export or import  
            if (from == "D") {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(3, 1) == "1").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            else if (from == "F") {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(3, 1) == "2").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }

            // Filter products by Origin
            if (origin == "VN")
            {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(4, 3) == "003").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            else if (origin == "IL")
            {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(4, 3) == "004").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            // Filter product by prices 

            if (maxPrice != 0){
                selectQuery = filterQuery.Where(x => x.CurrentPrice >= minPrice && x.CurrentPrice <= maxPrice).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();
            model.PageSize = pageSize != 0 ? pageSize : 12;


            if (style == "list"){
                ViewData["style"] = "list";
            }
            else {
                ViewData["style"] = "grid";
            }
            ViewData["categories"] = new String[] { "Hạt", "Rau củ", "Trái cây", "Nấm", "Socola", "Chùm ngây", "Thực phẩm sấy" };
            ViewData["catID"] = id;

            return View(model);
        }

        // GET: /Product/QuickView/<id>
        public IActionResult QuickView(int id)
        {
            ShopItem model = _dbContext.ShopItems.Find(id);

            return PartialView("_PartialQuickView", model);
        }

        public IActionResult Search(PagedList<ShopItem> model, string keyword, int pageSize, int id, string from, string origin, int maxPrice, int minPrice, string style) {
        
            var filterQuery = _dbContext.ShopItems.Where(x => x.Name.Contains(keyword));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            // Filter products by export or import  
            if (from == "D")
            {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(3, 1) == "1").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            else if (from == "F")
            {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(3, 1) == "2").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }

            // Filter products by Origin
            if (origin == "VN")
            {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(4, 3) == "003").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            else if (origin == "IL")
            {
                selectQuery = filterQuery.Where(x => x.SKU.Substring(4, 3) == "004").Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }
            // Filter product by prices 

            if (maxPrice != 0)
            {
                selectQuery = filterQuery.Where(x => x.CurrentPrice >= minPrice && x.CurrentPrice <= maxPrice).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);
            }

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();
            model.PageSize = pageSize != 0 ? pageSize : 12;

            ViewData["keyword"] = keyword;
            if (style == "list")
            {
                ViewData["style"] = "list";
            }
            else
            {
                ViewData["style"] = "grid";
            }
            ViewData["categories"] = new String[] { "Hạt", "Rau củ", "Trái cây", "Nấm", "Socola", "Chùm ngây", "Thực phẩm sấy" };

            return View(model);
        }

    }
}
