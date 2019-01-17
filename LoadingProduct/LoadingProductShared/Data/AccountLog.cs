using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCVShared.Data
{
    public class AccountLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Hoạt động")]
        public UserAction Action { get; set; }

        [Display(Name = "Địa chỉ IP")]
        [StringLength(60)]
        public string RemoteIP { get; set; }

        [Display(Name = "Dữ liệu")]
        public string LogData { get; set; }

        [Display(Name = "Create time")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime LogTime { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
    }
}
