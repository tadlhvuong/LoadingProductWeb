using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCVShared.Data
{
    public class UserThread
    {
        public int Id { get; set; }

        public int MsgCount { get; set; }

        public float TotalRating { get; set; }

        public DateTime? CreateTime { get; set; }

        public virtual ICollection<UserMessage> Children { get; set; }
    }

    public class UserMessage
    {
        public int Id { get; set; }

        [Display(Name = "Tác giả")]
        public int UserId { get; set; }

        [Display(Name = "Cấp trên")]
        public int ThreadId { get; set; }

        [Display(Name = "Cấp trên")]
        public int? ParentId { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [Display(Name = "Like")]
        public int Like { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? CreateTime { get; set; }

        public EntityStatus Status { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("ThreadId")]
        public virtual UserThread Thread { get; set; }

        [ForeignKey("ParentId")]
        public virtual UserMessage Parent { get; set; }

        public virtual ICollection<UserMessage> Children { get; set; }
    }
}
