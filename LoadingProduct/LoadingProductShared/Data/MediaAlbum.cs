using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCVShared.Data
{
    public class MediaAlbum
    {
        public int Id { get; set; }

        [Display(Name = "Tác giả")]
        public int? UserId { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "Tên code")]
        public string ShortName { get; set; }

        [Required, StringLength(128)]
        [Display(Name = "Tên gọi")]
        public string FullName { get; set; }

        [StringLength(256)]
        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        public virtual ICollection<MediaFile> MediaFiles { get; set; }
    }
}
