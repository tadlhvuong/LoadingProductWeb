using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TCVShared.Data
{
    public class AppSetting
    {
        [Key]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
