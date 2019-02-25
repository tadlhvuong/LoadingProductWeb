using LoadingProductShared.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LoadingProductWeb.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(32, ErrorMessage = "{0} phải có tối thiểu {2} ký tự.", MinimumLength = 3)]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ tài khoản")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class Login2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "{0} yêu cầu từ {2} đến {1} ký tự", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Mã xác thực")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Ghi nhớ thiết bị này")]
        public bool RememberMachine { get; set; }

        [Display(Name = "Ghi nhớ tài khoản")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class LoginWithRecoveryCodeViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Mã cứu hộ")]
        public string RecoveryCode { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class LoginExViewModel
    {
        public string Provider { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class TwoFactorAuthenticationViewModel
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }

    public class EnableAuthenticatorViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "{0} yêu cầu từ {2} đến {1} ký tự", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Mã xác thực")]
        public string Code { get; set; }

        [BindNever]
        public string SharedKey { get; set; }

        [BindNever]
        public string AuthenticatorUri { get; set; }
    }

    public class ShowRecoveryCodesViewModel
    {
        public string[] RecoveryCodes { get; set; }
    }

    public class UpdateUserModel
    {
        public int UserId { get; set; }

        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ mail")]
        public string Email { get; set; }

        [Display(Name = "Trạng thái")]
        public EntityStatus Status { get; set; }
    }

    public class MemberSearchModel
    {
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Số CMND")]
        public string IdCardNo { get; set; }

        [Display(Name = "Phương thức")]
        public int FindMode { get; set; }

        [Display(Name = "Kết quả")]
        public List<AppUser> Results { get; set; }
    }
}
