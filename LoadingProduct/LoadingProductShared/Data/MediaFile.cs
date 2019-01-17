using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TCVShared.Helpers;

namespace TCVShared.Data
{
    public class MediaFile
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }

        [StringLength(128)]
        [Display(Name = "Tên file")]
        public string FileName { get; set; }

        [StringLength(512)]
        [Display(Name = "Đường dẫn")]
        public string FullPath { get; set; }

        [Display(Name = "Dung lượng")]
        public long FileSize { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [ForeignKey("AlbumId")]
        public virtual MediaAlbum MediaAlbum { get; set; }

        [NotMapped]
        [StringLength(512)]
        [Display(Name = "URL tải file")]
        public string FileLink { get; set; }
    }
}
