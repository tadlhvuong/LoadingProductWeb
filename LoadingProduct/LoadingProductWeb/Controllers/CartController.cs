using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using System.Linq;
using System.Collections.Generic;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCVWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<CartController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public CartController(AppDBContext dbContext,
                                ILogger<CartController> logger,
                                UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;

        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");
            if (cart == null)
            {
                cart = new ShopCart();
                cart.Items = new List<CartItem>();
            }

            var suggestionProducts = _dbContext.ShopItems.Where(product => product.SKU.Substring(6, 1) == "S").Take(4).ToList(); ;

            ViewData["suggestionProducts"] = suggestionProducts;

                return View(cart);
        }

        public IActionResult Checkout()
        {
            return View();
        }

        // Cart/AddToCart/1?quantity=1 
        public ActionResult AddToCart(int id, int quantity) {
            var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");

            // Set default quantity 
            if (quantity == 0) 
                quantity = 1;

            if (cart == null) {
                cart = new ShopCart();
                var cartItem = new CartItem();
                var shopItem = _dbContext.ShopItems.Find(id);

                cartItem.ShopItem = shopItem;
                cartItem.Quantity = quantity;
                cartItem.ShopCart = cart;

                cart.Items = new List<CartItem>();
                cart.Items.Add(cartItem);

                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            else {
                int existIndex = -1;
                for (int i = 0; i < cart.Items.Count; i++)
                {
                    if (cart.Items.ElementAt(i).ShopItem.Id == id)
                    {
                        existIndex = i;
                        break;
                    }
                }
                if (existIndex == -1){
                    var cartItem = new CartItem();
                    var shopItem = _dbContext.ShopItems.Find(id);
                    cartItem.ShopItem = shopItem;
                    cartItem.Quantity = quantity;
                    cartItem.ShopCart = cart;
                    cart.Items.Add(cartItem);
                }
                else {
                    cart.Items.ElementAt(existIndex).Quantity = cart.Items.ElementAt(existIndex).Quantity + quantity;
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);

            }
            return PartialView("_PartialHeader");
        }

        public IActionResult AddToCartFromCart(int id, int quantity) {
            this.AddToCart(id, quantity);
            
            return RedirectToAction("Index", "Cart"); 
        }

        // /Cart/RemoveProduct/1
        public ActionResult RemoveProduct(int id){
            var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");
            var indexOfProduct = this.FindIndexOfProduct(id, cart);
            cart.Items.Remove(cart.Items.ElementAt(indexOfProduct));

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return PartialView("_PartialHeader");
        }

        // /Cart/ClearCart
        public ActionResult ClearCart(){
            var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");
            cart.Items.Clear();

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return PartialView("_PartialHeader");
        }

        // /Cart/IncreaseQuantity/1
        public ActionResult IncreaseQuantity(int id){
            var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");
            var indexOfProduct = this.FindIndexOfProduct(id, cart);
            cart.Items.ElementAt(indexOfProduct).Quantity++ ;

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return PartialView("_PartialHeader");
        }

        // /Cart/DecreaseQuantity/1
        public ActionResult DecreaseQuantity(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");
            var indexOfProduct = this.FindIndexOfProduct(id, cart);
            cart.Items.ElementAt(indexOfProduct).Quantity--;

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return PartialView("_PartialHeader");
        }

        //CUSTOM FUNCTION 
        private int FindIndexOfProduct(int id, ShopCart cart){
            int existIndex = -1;
            for (int i = 0; i < cart.Items.Count; i++)
            {
                if (cart.Items.ElementAt(i).ShopItem.Id == id)
                {
                    existIndex = i;
                    return i;
                }
            }
            return -1;
        }
    }
}
