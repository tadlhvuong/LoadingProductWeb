using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TCVShared.Data
{
    public class Supplier
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Display(Name = "Bình luận")]
        public int? ThreadId { get; set; }

        [ForeignKey("ThreadId")]
        public virtual UserThread Thread { get; set; }

        [Display(Name = "Nhà cung cấp")]
        public string Name { get; set; }

        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }

        [StringLength(256)]
        [Display(Name = "Địa chỉ")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [StringLength(64)]
        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [StringLength(64)]
        [Display(Name = "Tiểu bang")]
        public string State { get; set; }

        [StringLength(64)]
        [Display(Name = "Quốc gia")]
        public string Country { get; set; }

        [StringLength(32)]
        [Display(Name = "Mã zip")]
        public string ZipCode { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [NotMapped]
        [Display(Name = "Bình luận")]
        public virtual ICollection<UserMessage> Messages
        {
            get
            {
                if (ThreadId == null)
                    return new List<UserMessage>();

                return Thread.Children.Where(x => x.ParentId == null).ToList();
            }
        }
        [NotMapped]
        [Display(Name = "Tổng số bình luận")]
        public virtual int TotalMessages
        {
            get
            {
                return Thread == null ? 0 : Thread.MsgCount;
            }
        }
    }
}
