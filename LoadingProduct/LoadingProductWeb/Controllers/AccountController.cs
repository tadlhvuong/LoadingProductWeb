using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//HEAD
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
//cd4f44d2c61f2a17ec907d90a0de78081c081231
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using TCVWeb.Areas.Admin.Controllers;
using TCVWeb.Models;

namespace TCVWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AccountController> logger,
            AppDBContext dbContex)
        {
            _dbContext = dbContex;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if(User.IsInRole("Admin"))
                    {
                        return RedirectToAction(nameof(HomeAdminController.Index), "HomeAdmin", new { area = "Admin" });
                    }
                    else {
                        if (_dbContext.Users.Where(user => user.Email == model.Email).FirstOrDefault().EmailConfirmed == false)
                        {
                            await _signInManager.SignOutAsync();
                            ViewData["StatusMessage"] = "Đăng nhập thất bại, bạn phải kích hoạt tài khoản sau khi đăng ký.";
                            return View(model);
                        }
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("SendCode", new { returnUrl = model.ReturnUrl, rememberMe = false });

                }
                if (result.IsLockedOut)
                {
                    ViewData["StatusMessage"] = "Tài khoản bị khóa đăng nhập, xin thử lại sau 5 phút.";
                }
                else
                {
                    ViewData["StatusMessage"] = "Đăng nhập thất bại, xin kiểm tra lại.";
                }
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginCode(LoginWithRecoveryCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản bị khóa đăng nhập, xin thử lại sau 5 phút.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Mã cứu hộ không chính xác, vui lòng nhập mã khác.");
                return View(model);
            }
        }

        private async Task<string> GetUserName(ExternalLoginInfo loginInfo)
        {
            string defaultName = null;
            if (loginInfo.LoginProvider == "Facebook")
            {
                var nameClaim = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                    defaultName = Common.NormalizeVietnamese(nameClaim.Value);
            }

            if (defaultName == null)
                return null;

            string newUserName = defaultName;
            for (int i = 0; i < 30; i++)
            {
                AppUser newUser = await _userManager.FindByNameAsync(newUserName);
                if (newUser == null)
                    break;

                int randNo = Common.Random(99) + 1;
                newUserName = string.Format("{0}{1:D2}", defaultName, randNo);
            }

            return newUserName;
        }

        private async Task<AppUser> CreateUserEx(ExternalLoginInfo loginInfo)
        {
            string newUserName = await GetUserName(loginInfo);
            if (newUserName == null)
                return null;

            var exEmail = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            AppUser appUser = new AppUser
            {
                UserName = newUserName,
                Email = exEmail?.Value,
                EmailConfirmed = (exEmail != null),
                CreateTime = DateTime.Now,
                LastUpdate = DateTime.Now,
                Status = EntityStatus.Enabled
            };

            var result = await _userManager.CreateAsync(appUser);
            if (!result.Succeeded)
            {
                return null;
            }

            result = await _userManager.AddLoginAsync(appUser, loginInfo);
            if (!result.Succeeded)
            {
                return null;
            }

            return appUser;
        }

        //public IActionResult ExternalLogin(LoginExViewModel model)
        //{
        //    var redirectUrl = Url.Action(nameof(LoginExCallback), "Account", new { model.ReturnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(model.Provider, redirectUrl);
        //    return Challenge(properties, model.Provider);
        //}

        //public async Task<IActionResult> LoginExCallback(string returnUrl = null, string remoteError = null)
        //{
        //    if (remoteError != null)
        //    {
        //        ErrorMessage = $"Đăng nhập Facebook/Google lỗi: {remoteError}";
        //        return RedirectToAction(nameof(Login));
        //    }

        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //        return RedirectToAction(nameof(Login));

        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        ErrorMessage = "Tài khoản bị khóa đăng nhập, xin thử lại sau 5 phút.";
        //        return RedirectToAction(nameof(Login));
        //    }
        //    else
        //    {
        //        var appUser = await CreateUserEx(info);
        //        if (appUser == null)
        //        {
        //            ErrorMessage = "Lỗi: Không thể tạo Tài khoản liên kết.";
        //            return RedirectToAction(nameof(Login));
        //        }

        //        await _signInManager.SignInAsync(appUser, true);
        //        return RedirectToLocal(returnUrl);
        //    }
        //}

        [HttpGet]
        public IActionResult AccessDenied()
        {
            ErrorMessage = "Bạn không có quyền truy cập trang này.";
            return RedirectToAction(nameof(Login));
        }


        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {userName} was created.", model.Email);
                    //Bug: Remember browser option missing?
                    //Uncomment this and comment the later part if account verification is not needed.
                    //await SignInManager.SignInAsync(user, isPersistent: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await MessageServices.SendEmailAsync(model.Email, "TiCiVi - Xác nhận tài khoản",
                                                         "<div> <p> Xin chào, cám ơn bạn đã đăng ký tài khoản trên TiCiVi ! </p> " + 
                                                                "<p> Vì lý do bảo mật, hãy ấn vào link dưới đây để xác thực địa chỉ email của ban. </p>" +
                                                                "<a href=\"" + callbackUrl + "\"> Ấn vào đây </a> " +
                                                                "<p> Cám ơn bạn! </p> </br>" +
                                                                "<p> Đội ngũ TiCiVi </p>" +
                                                                " </div>");

                    return RedirectToAction("RegisterConfirmation", "Account", new { link = callbackUrl });
                }
                AddErrors(result);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/RegisterConfirmation
        public ActionResult RegisterConfirmation(string link)
        {
            return View();
        }


        //
        // GET: /Account/ConfirmEmail
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account
        public ActionResult Edit()
        {
            return View();
        }

        //
        // POST: /Account/Login
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to lockoutOnFailure: true
        //    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("Logged in {userName}.", model.Email);

        //        if (returnUrl == "/Checkout")
        //        {
        //            return RedirectToAction("Index", "Checkout");
        //        }

        //        return RedirectToAction("Index", "Home");
        //    }
        //    if (result.RequiresTwoFactor)
        //    {
        //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        return View("Lockout");
        //    }
        //    else
        //    {
        //        _logger.LogWarning("Failed to log in {userName}.", model.Email);
        //        ModelState.AddModelError("", "Invalid login attempt.");
        //        return View(model);
        //    }
        //}

        //
        // Get: /Account/Logout
        public async Task<ActionResult> Logout()
        {
            var userName = HttpContext.User.Identity.Name;
            // clear all items from the cart
            HttpContext.Session.Clear();

            await _signInManager.SignOutAsync();

            //// TODO: Currently SignInManager.SignOut does not sign out OpenIdc and does not have a way to pass in a specific
            //// AuthType to sign out.
            //var appEnv = HttpContext.RequestServices.GetService<IHostingEnvironment>();
            //if (appEnv.EnvironmentName.StartsWith("OpenIdConnect"))
            //{
            //    return new SignOutResult("OpenIdConnect", new AuthenticationProperties
            //    {
            //        RedirectUri = Url.Action("Index", "Home")
            //    });
            //}

            _logger.LogInformation("{userName} logged out.", userName);
            return RedirectToAction("Index", "Home");
        }



        //
        // Get: /Account/ForgotPassword
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code }, protocol: HttpContext.Request.Scheme);
                await MessageServices.SendEmailAsync(model.Email, "Reset Password",
                    "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError("", string.Format("We could not locate an account with email : {0}", model.Email));

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();////
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(string code)
        {
            //TODO: Fix this?
            var resetPasswordViewModel = new ResetPasswordViewModel() { Code = code };
            return code == null ? View("Error") : View(resetPasswordViewModel);
        }


        //POST: /Account/ResetPassword
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToAction(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.LoginProvider;
                // REVIEW: handle case where email not in claims?
                var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        ////
        //// POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);

                // NOTE: Used for end to end testing only
                //Just for automated testing adding a claim named 'ManageStore' - Not required for production
                var manageClaim = info.Principal.Claims.Where(c => c.Type == "ManageStore").FirstOrDefault();
                if (manageClaim != null)
                {
                    await _userManager.AddClaimAsync(user, manageClaim);
                }

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(bool rememberMe, string returnUrl = null)
        {
            //TODO : Default rememberMe as well?
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await MessageServices.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await MessageServices.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Remove before production
#if DEMO
                    if (user != null)
                    {
                        ViewBag.Code = await UserManager.GenerateTwoFactorTokenAsync(user, provider);
                    }
#endif
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        //public async Task<IActionResult> Login(string returnUrl = null)
        //{
        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //    return RedirectToAction(nameof(Login), new { area = "Admin" });
        //}

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }

        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                _logger.LogWarning("Error in creating user: {error}", error.Description);
            }
        }

        private Task<AppUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }





}