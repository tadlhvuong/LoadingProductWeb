using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using TCVWeb.Models;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCVWeb.Controllers
{
    [Authorize]
    public class WishController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<WishController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public WishController(AppDBContext dbContext,
    ILogger<WishController> logger,
    UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;

        }

        // GET: /Wish
        public IActionResult Index()
        {
            var userId = Int32.Parse(_userManager.GetUserId(HttpContext.User));
            var wish = _dbContext.ShopWishes.Where(w => w.UserId == userId).ToList();

            if (wish.Count == 0)
            {
                ShopWish userWishlist = new ShopWish
                {
                    UserId = userId
                };
                _dbContext.ShopWishes.Add(userWishlist);
                _dbContext.SaveChanges();
            }

            ShopWish myWishList = _dbContext.ShopWishes.Where(w => w.UserId == userId).First();
            List<WishItem> wishItemsDB = _dbContext.WishItems.Where(item => item.WishId == myWishList.Id).ToList();
            List<WishItem> wishItems = new List<WishItem>();

            foreach (var wishItemDB in wishItemsDB){
                ShopItem shopItem = _dbContext.ShopItems.Find(wishItemDB.ItemId);
                wishItemDB.ShopItem = shopItem;
                wishItemDB.ShopWish = myWishList;

                wishItems.Add(wishItemDB);
            }

            myWishList.Items = wishItems;

            return View(myWishList);
        }

        // GET: /AddProductToWishlist/<id>
        [AllowAnonymous]
        public String AddProductToWishlist(int id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var userId = Int32.Parse(_userManager.GetUserId(HttpContext.User));
                var wish = _dbContext.ShopWishes.Where(w => w.UserId == userId).ToList();

                if (wish.Count == 0)
                {
                    ShopWish userWishlist = new ShopWish
                    {
                        UserId = userId
                    };
                    _dbContext.ShopWishes.Add(userWishlist);
                    _dbContext.SaveChanges();
                }

                ShopWish wish2 = _dbContext.ShopWishes.Where(w => w.UserId == userId).First();

                var result = _dbContext.WishItems.SingleOrDefault(b => b.ItemId == id && b.WishId == wish2.Id);
                if (result == null)
                {
                    WishItem item = new WishItem
                    {

                        ItemId = id,
                        WishId = wish2.Id
                    };

                    _dbContext.WishItems.Add(item);
                    _dbContext.SaveChanges();
                    return "Thêm sản phẩm vào danh sách yêu thích thành công";
                }
                else {
                    return "Sản phẩm đã tồn tại trong danh sách yêu thích của bạn";
                }
            }
            else {
                return "Bạn phải đăng nhập để lưu được những sản phẩm yêu thích";
            }
        }



        // /Cart/RemoveFromWishList/<id>
        public void RemoveProductFromWishlish(int id)
        {
            var userId = Int32.Parse(_userManager.GetUserId(HttpContext.User));
            ShopWish wish2 = _dbContext.ShopWishes.Where(w => w.UserId == userId).First();
            var result = _dbContext.WishItems.SingleOrDefault(b => b.ItemId == id && b.WishId == wish2.Id);
            if (result != null)
            {
                _dbContext.WishItems.Remove(result);
                _dbContext.SaveChanges();
            }
        }
    }
}
