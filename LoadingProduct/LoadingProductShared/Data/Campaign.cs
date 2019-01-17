using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TCVShared.Data
{
    public class Campaign
    {
        public int Id { get; set; }

        [Required, StringLength(64)]
        public string Code { get; set; }

        [StringLength(128)]
        [Display(Name = "Tên đầy đủ")]
        public string FullName { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }
    }
}
