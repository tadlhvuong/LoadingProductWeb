using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LoadingProductShared.Data
{
    public class Banner
    {
        public int Id { get; set; }

        [Display(Name = "Hình ảnh")]
        public int? AlbumId { get; set; }

        [StringLength(128)]
        [Display(Name = "Người tạo")]
        public string CreateUser { get; set; }

        [StringLength(128)]
        [Display(Name = "Người sửa")]
        public string UpdateUser { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Sửa cuối")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Giờ đăng")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? PublishTime { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [StringLength(128)]
        [Display(Name = "Tiêu đề English")]
        public string TitleEn { get; set; }

        [StringLength(512)]
        [Display(Name = "Hình đại diện")]
        public string Image { get; set; }

        [StringLength(1024)]
        [Display(Name = "Giới thiệu")]
        [DataType(DataType.MultilineText)]
        public string Preview { get; set; }

        [StringLength(1024)]
        [Display(Name = "Giới thiệu English")]
        [DataType(DataType.MultilineText)]
        public string PreviewEn { get; set; }
    }
}
