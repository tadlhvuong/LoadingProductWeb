using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TCVWeb.Models;
using TCVShared.Data;
using System.Threading;
using System.Collections.Generic;

namespace TCVWeb.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly AppDBContext DBContext;
        private readonly UserManager<AppUser> UserManager;
        private readonly SignInManager<AppUser> SignInManager;
        private IAuthenticationSchemeProvider SchemeProvider;
        public ManageController(
            AppDBContext dBContext,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuthenticationSchemeProvider schemes)
        {
            DBContext = dBContext;
            UserManager = userManager;
            SignInManager = signInManager;
            SchemeProvider = schemes;
        }
        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                       : message == ManageMessageId.SetAddressSuccess ? "Your address has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.UnsetTwoFactorSuccess ? "Your two-factor authentication provider has been remove."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var user = await GetCurrentUserAsync();
            //AppUser user = UserManager.Users.Where(u => u.Email == this.User.Identity.Name).First();

            var model = new IndexViewModel
            {
                HasPassword = await UserManager.HasPasswordAsync(user),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(user),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user),
                //Logins = await UserManager.GetLoginsAsync(user),
                //BrowserRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user)
            };

            var shippingAddressList = DBContext.Shippings.Where(address => address.UserId == user.Id).ToList();
            if (shippingAddressList.Count != 0){
                model.ShippingAddress = shippingAddressList.First();
            }

            return View(model);
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await UserManager.SetTwoFactorEnabledAsync(user, true);
                // TODO: flow remember me somehow?
                await SignInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.SetTwoFactorSuccess });
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await UserManager.SetTwoFactorEnabledAsync(user, false);
                await SignInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.UnsetTwoFactorSuccess });
        }

        public async Task<IActionResult> GetMyOrder(MyOrdersViewModel model){
            var user = await GetCurrentUserAsync();
            var orders = DBContext.ShopOrders.Where(order => order.UserId == user.Id).ToList();
            
            foreach (var order in orders) {
                var orderItems = DBContext.OrderItems.Where(orderItem => orderItem.OrderId == order.Id).ToList();
                foreach (var orderItem in orderItems) {
                    orderItem.ShopItem = DBContext.ShopItems.Find(orderItem.ItemId);
                }
                order.Items = orderItems;
            }

            model.orders = orders;

            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public async Task<IActionResult> ChangePassword()
        {
            var user = await GetCurrentUserAsync();
            var model = new ChangePasswordViewModel();            

            var shippingAddressList = DBContext.Shippings.Where(address => address.UserId == user.Id).ToList();
            if (shippingAddressList.Count != 0)
            {
                model.ShippingAddress = shippingAddressList.First();
            }

            return View(model);
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken requestAborted)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();

            if (model.ShippingAddress == null) {
                if (user != null)
                {
                    var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    AddErrors(result);
                    return View(model);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            else {
                Shipping shippingAddress = model.ShippingAddress;
                shippingAddress.UserId = user.Id;

                var result = DBContext.Shippings.SingleOrDefault(b => b.UserId == user.Id);
                if (result != null)
                {
                    result.City = shippingAddress.City;
                    result.State = shippingAddress.State;
                    result.Address = shippingAddress.Address;

                    DBContext.SaveChanges();
                }
                else {
                    DBContext.Shippings.Add(shippingAddress);
                    await DBContext.SaveChangesAsync(requestAborted);
                }

                return RedirectToAction("Index", new { Message = ManageMessageId.SetAddressSuccess });
            }
        }

        //
        // GET: /Manage/SetPassword
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await UserManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Manage/RememberBrowser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RememberBrowser()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await SignInManager.RememberTwoFactorClientAsync(user);
                await SignInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/ForgetBrowser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetBrowser()
        {
            await SignInManager.ForgetTwoFactorClientAsync();
            return RedirectToAction("Index", "Manage");
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            SetAddressSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            UnsetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<AppUser> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}