using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TCVShared.Data
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole()
        {
        }

        public AppRole(string roleName)
            : base(roleName)
        {
        }
    }

    public class AppUser : IdentityUser<int>
    {
        [Display(Name = "Đã khóa")]
        public bool IsLocked { get; set; }

        [Display(Name = "Thời gian mở")]
        public DateTime? OpenTime { get; set; }

        [Display(Name = "Trạng thái")]
        public EntityStatus Status { get; set; }

        [Display(Name = "Khởi tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "IP khởi tạo")]
        [StringLength(60)]
        public string CreateIP { get; set; }

        [Display(Name = "Đăng nhập")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "IP đăng nhập")]
        [StringLength(60)]
        public string LastLoginIP { get; set; }

        [Display(Name = "Cập nhật")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? LastUpdate { get; set; }

        [StringLength(128)]
        [Display(Name = "Quản trị")]
        public string UpdateUser { get; set; }

        //public virtual ICollection<CashChargeLog> CashChargeLogs { get; set; }

        //public virtual ICollection<GiftCodeData> GiftCodeDatas { get; set; }

        public virtual ICollection<AccountLog> AccountLogs { get; set; }

        [NotMapped]
        public UserMode UserMode
        {
            get
            {
                if (!EmailConfirmed && !PhoneNumberConfirmed)
                    return UserMode.None;

                if (!IsLocked)
                    return UserMode.Unlocked;

                if (OpenTime == null)
                    return UserMode.Locked;

                if (OpenTime > DateTime.Now)
                    return UserMode.Unlocking;
                else
                    return UserMode.Unlocked;
            }
        }

        [NotMapped]
        public virtual ICollection<UserLoginInfo> ExtLogins { get; set; }

        [NotMapped]
        public string AvatarImg
        {
            get
            {
                if (ExtLogins == null)
                    return "/img/avatar.jpg";

                var fbLogin = ExtLogins.FirstOrDefault(x => x.LoginProvider == "Facebook");
                if (fbLogin == null)
                    return "/img/avatar.jpg";

                return string.Format("https://graph.facebook.com/{0}/picture?type=large", fbLogin.ProviderKey);
            }
        }

        [NotMapped]
        public string FacebookUrl
        {
            get
            {
                if (ExtLogins == null)
                    return null;

                var fbLogin = ExtLogins.FirstOrDefault(x => x.LoginProvider == "Facebook");
                if (fbLogin == null)
                    return null;

                return string.Format("https://www.facebook.com/app_scoped_user_id/{0}", fbLogin.ProviderKey);
            }
        }
    }
}
