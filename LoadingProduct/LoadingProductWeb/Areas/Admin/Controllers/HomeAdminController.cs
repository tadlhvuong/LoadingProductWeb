using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVWeb.Areas.Admin.Models;
using TCVWeb.Models;

namespace TCVWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Manager,Operator")]
    public class HomeAdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly AppDBContext _dbContext;
        private readonly ILogger _logger;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public HomeAdminController(
            UserManager<AppUser> userManager,
            UrlEncoder urlEncoder,
            AppDBContext dbContext,
            ILogger<HomeAdminController> logger)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(appUser) != null,
                Is2faEnabled = appUser.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(appUser),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                throw new ApplicationException($"Unable to load appUser with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(appUser, model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                throw new ApplicationException($"Unable to load appUser with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(appUser, model);
                return View(model);
            }

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                appUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(appUser, model);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(appUser, true);

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, 10);

            var xModel = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };
            return View("ShowRecoveryCodes", xModel);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                throw new ApplicationException($"Unable to load appUser with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(appUser, false);
            await _userManager.ResetAuthenticatorKeyAsync(appUser);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                throw new ApplicationException($"Unable to load appUser with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!appUser.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for appUser with ID '{appUser.Id}' because they do not have 2FA enabled.");
            }

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                throw new ApplicationException($"Unable to load appUser with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!appUser.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for appUser with ID '{appUser.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, 10);

            var xModel = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };
            return View("ShowRecoveryCodes", xModel);
        }

        #region Helpers

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            string appName = string.Format("TCV.Admin.{0}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode(appName),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(AppUser appUser, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(appUser);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(appUser);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(appUser);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(appUser.Email, unformattedKey);
        }

        #endregion
    }
}