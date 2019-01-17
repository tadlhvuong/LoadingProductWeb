using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TCVShared.Data;
using System;

namespace TCVWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
        //[TempData]
        //public string ErrorMessage { get; set; }

        //[HttpGet]
        //public async Task<IActionResult> Login(string returnUrl = null)
        //{
        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction(nameof(HomeAdminController.Index), "HomeAdmin");
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToAction(nameof(Login2fa), new { model.ReturnUrl, model.RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            ModelState.AddModelError(string.Empty, "Tài khoản bị khóa đăng nhập, xin thử lại sau 5 phút.");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Đăng nhập thất bại, xin kiểm tra lại.");
        //        }
        //    }

        //    return View(model);
        //}

        //public async Task<IActionResult> Login2fa(bool rememberMe, string returnUrl = null)
        //{
        //    var appUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (appUser == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication appUser.");
        //    }

        //    var model = new Login2faViewModel { RememberMe = rememberMe, ReturnUrl = returnUrl };
        //    return View(model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login2fa(Login2faViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var appUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (appUser == null)
        //    {
        //        throw new ApplicationException($"Unable to load appUser with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        //    var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

        //    if (result.Succeeded)
        //    {
        //        return RedirectToLocal(model.ReturnUrl);
        //    }
        //    else if (result.IsLockedOut)
        //    {
        //        ModelState.AddModelError(string.Empty, "Tài khoản bị khóa đăng nhập, xin thử lại sau 5 phút.");
        //        return View(model);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Mã xác thực không chính xác.");
        //        return View(model);
        //    }
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> LoginCode(string returnUrl = null)
        //{
        //    // Ensure the user has gone through the username & password screen first
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    var model = new LoginWithRecoveryCodeViewModel { ReturnUrl = returnUrl };
        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LoginCode(LoginWithRecoveryCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load two-factor authentication user.");
        //    }

        //    var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

        //    var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        //    if (result.Succeeded)
        //    {
        //        return RedirectToLocal(model.ReturnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        ModelState.AddModelError(string.Empty, "Tài khoản bị khóa đăng nhập, xin thử lại sau 5 phút.");
        //        return View(model);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Mã cứu hộ không chính xác, vui lòng nhập mã khác.");
        //        return View(model);
        //    }
        //}

        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction(nameof(AccountController.Login), new { Areas = "Admin" });
        //}

        //private async Task<string> GetUserName(ExternalLoginInfo loginInfo)
        //{
        //    string defaultName = null;
        //    if (loginInfo.LoginProvider == "Facebook")
        //    {
        //        var nameClaim = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        //        if (nameClaim != null)
        //            defaultName = Common.NormalizeVietnamese(nameClaim.Value);
        //    }

        //    if (defaultName == null)
        //        return null;

        //    string newUserName = defaultName;
        //    for (int i = 0; i < 30; i++)
        //    {
        //        AppUser newUser = await _userManager.FindByNameAsync(newUserName);
        //        if (newUser == null)
        //            break;

        //        int randNo = Common.Random(99) + 1;
        //        newUserName = string.Format("{0}{1:D2}", defaultName, randNo);
        //    }

        //    return newUserName;
        //}

        //private async Task<AppUser> CreateUserEx(ExternalLoginInfo loginInfo)
        //{
        //    string newUserName = await GetUserName(loginInfo);
        //    if (newUserName == null)
        //        return null;

        //    var exEmail = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        //    AppUser appUser = new AppUser
        //    {
        //        UserName = newUserName,
        //        Email = exEmail?.Value,
        //        EmailConfirmed = (exEmail != null),
        //        CreateTime = DateTime.Now,
        //        LastUpdate = DateTime.Now,
        //        Status = EntityStatus.Enabled
        //    };

        //    var result = await _userManager.CreateAsync(appUser);
        //    if (!result.Succeeded)
        //    {
        //        return null;
        //    }

        //    result = await _userManager.AddLoginAsync(appUser, loginInfo);
        //    if (!result.Succeeded)
        //    {
        //        return null;
        //    }

        //    return appUser;
        //}

        //public IActionResult LoginEx(LoginExViewModel model)
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

        //[HttpGet]
        //public IActionResult AccessDenied()
        //{
        //    ErrorMessage = "Bạn không có quyền truy cập trang này.";
        //    return RedirectToAction(nameof(Login));
        //}

        //#region Helpers

        //private IActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (string.IsNullOrEmpty(returnUrl))
        //        return RedirectToAction(nameof(HomeAdminController.Index), "Home");

        //    return Redirect(returnUrl);
        //}

        //#endregion
    }
}