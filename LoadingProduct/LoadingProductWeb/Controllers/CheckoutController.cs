using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using Microsoft.AspNetCore.Identity;
using TCVShared.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCVWeb.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private const string PromoCode = "FREE";

        private readonly AppDBContext _dbContext;
        private readonly ILogger<CartController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(AppDBContext dbContext,
                                ILogger<CartController> logger,
                                UserManager<AppUser> userManager
                                 )
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: /Checkout/
        [AllowAnonymous]
        public IActionResult Index(ShopOrder model)
        {
            ShopCart cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");

            if (cart == null) {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            if (cart == null){
                return RedirectToAction("Index", "Cart");
            }
            model.AdjustPrice = cart.SubTotal;
            model.GrandTotalPrice = cart.GrandTotal;
            model.OrderStatus = 0;
            model.ShippingFee = cart.ShippingFee;

            List<OrderItem> listOrder = new List<OrderItem>();

            foreach(var cartItem in cart.Items){
                OrderItem orderItem = new OrderItem();

                orderItem.ShopItem = cartItem.ShopItem;
                orderItem.Quantity = cartItem.Quantity;
                orderItem.ItemId = cartItem.ItemId;
                orderItem.ShopOrder = model;

                listOrder.Add(orderItem);
            }

            model.Items = listOrder;

            if (this.User.Identity.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var shippingAddressList = _dbContext.Shippings.Where(address => address.UserId == Int32.Parse(userId)).ToList();
                if (shippingAddressList.Count != 0)
                {
                    model.ShippingAddress = shippingAddressList.First();
                }

                model.AppUser = _userManager.Users.Where(user => user.Email == this.User.Identity.Name).First();
            }

            return View(model);
        }

        // Post : /Checkout/PlaceOrder
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PlaceOrder(ShopOrder model, CancellationToken requestAborted)
        {
            ShopCart cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");

            model.AdjustPrice = cart.SubTotal;
            model.GrandTotalPrice = cart.GrandTotal;
            model.OrderStatus = 0;
            model.ShippingFee = cart.ShippingFee;
            model.CreateTime = DateTime.Now;

            var userID = _userManager.GetUserId(HttpContext.User);
            if (userID != null)
            {
                model.UserId = int.Parse(userID);
            }
            else
            {
                model.UserId = 0;
            }

            _dbContext.ShopOrders.Add(model);


            List<OrderItem> listOrder = new List<OrderItem>();

            foreach (var cartItem in cart.Items)
            {
                OrderItem orderItem = new OrderItem();

                //orderItem.ShopItem = cartItem.ShopItem;
                orderItem.Quantity = cartItem.Quantity;
                orderItem.ItemId = cartItem.ShopItem.Id;
                orderItem.OrderId = model.Id;
                //orderItem.ShopOrder = model;

                listOrder.Add(orderItem);

                _dbContext.OrderItems.Add(orderItem);
            }
            //model.Items = listOrder;


            await _dbContext.SaveChangesAsync(requestAborted);


            return View(model);
        }

        //// GET : /Checkout/ApplyShippingFee
        //[AllowAnonymous]
        //public ActionResult ApplyShippingFee(string province, string district, string street) {
        //    var cart = HttpContext.Session.GetObjectFromJson<ShopCart>("Cart");
        //    if (province == "HCM"){
        //        if (district == "1" || district == "2"){
        //            cart.ShippingFee = 0.0;
        //        }
        //        else {
        //            cart.ShippingFee = 30000;
        //        }
        //    }
        //    HttpContext.Session.SetObjectAsJson("Cart", cart);

        //    return PartialView("_PartialTotal", cart);
        //}

    }

}
