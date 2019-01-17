using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TCVShared.Data
{

    public class ShopItemTaxo
    {
        public int Id { get; set; }

        public int TaxoId { get; set; }

        public int ItemId { get; set; }

        [ForeignKey("TaxoId")]
        public virtual Taxonomy Taxonomy { get; set; }

        [ForeignKey("ItemId")]
        public virtual ShopItem ShopItem { get; set; }
    }

    public class ShopItem
    {
        public int Id { get; set; }

        [StringLength(32)]
        [Display(Name = "Mã hàng")]
        public string SKU { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "Tên gọi")]
        public string Name { get; set; }

        [StringLength(64)]
        [Display(Name = "Tên gọi English")]
        public string NameEn { get; set; }

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

        [Display(Name = "Mô tả")]
        public string Content { get; set; }
        [Display(Name = "Mô tả English")]
        public string ContentEn { get; set; }

        [Display(Name = "Giá ban đầu")]
        [DisplayFormat(DataFormatString = "{0:#,#} VNĐ")]
        public double RegularPrice { get; set; }
        [Display(Name = "Giá ban đầu English")]
        [DisplayFormat(DataFormatString = "$ {0:#,#.00}")]
        public double RegularPriceEn { get; set; }

        [Display(Name = "Giá khuyến mãi")]
        [DisplayFormat(DataFormatString = "{0:#,#} VNĐ")]
        public double SalePrice { get; set; }
        [Display(Name = "Giá khuyến mãi English")]
        [DisplayFormat(DataFormatString = "$ {0:#,#.00}")]
        public double SalePriceEn { get; set; }

        [Display(Name = "Đóng gói")]
        public string Packaging { get; set; }
        [Display(Name = "Đóng gói English")]
        public string PackagingEn { get; set; }

        [Display(Name = "Thông số")]
        public string Specifications { get; set; }
        [Display(Name = "Thông số English")]
        public string SpecificationsEn { get; set; }

        [Display(Name = "Hình ảnh")]
        public int? AlbumId { get; set; }

        [Display(Name = "Bình luận")]
        public int? ThreadId { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Sửa cuối")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Giờ đăng")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? PublishTime { get; set; }

        [Display(Name = "Trạng thái")]
        public EntityStatus Status { get; set; }

        [ForeignKey("ThreadId")]
        public virtual UserThread Thread { get; set; }

        [ForeignKey("AlbumId")]
        public virtual MediaAlbum MediaAlbum { get; set; }

        [Display(Name = "Phân loại")]
        public virtual ICollection<ShopItemTaxo> Taxonomies { get; set; }

        [Display(Name = "Thuộc tính")]
        public virtual ICollection<ShopItemAttrib> Attributes { get; set; }

        [NotMapped]
        [Display(Name = "Phân loại")]
        public int[] ItemCats { get; set; }

        [NotMapped]
        [Display(Name = "Thẻ gắn")]
        public string ItemTags { get; set; }

        [NotMapped]
        [Display(Name = "Xuất xứ")]
        public int[] Exports { get; set; }

        [NotMapped]
        [Display(Name = "Nước sản xuất")]
        public int[] ExportsPlace { get; set; }

        [NotMapped]
        [Display(Name = "Kích thước sản phẩm")]
        public int[] SizeProduct { get; set; }

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
        [Display(Name = "Tổng số Bình luận")]
        public virtual int TotalMessages
        {
            get
            {
                return Thread == null ? 0 : Thread.MsgCount;
            }
        }

        [NotMapped]
        [Display(Name = "Rating")]
        public virtual float TotalRating
        {
            get
            {
                return Thread == null ? 0 : Thread.TotalRating/5;
            }
        }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,#.000} VNĐ")]
        public virtual double CurrentPrice
        {
            get
            {
                if (SalePrice > 0)
                    return SalePrice;

                return RegularPrice;
            }
        }               


        [NotMapped]
        public virtual bool NewProduct
        {
            get
            {
                if (CreateTime == null)
                    return false;

                var diff = DateTime.Now - CreateTime.Value;
                return (diff.TotalHours < 168);
            }
        }

        [NotMapped]
        public virtual string DiscountPercent
        {
            get
            {
                if (this.SalePrice != this.RegularPrice) {
                    double number = (this.RegularPrice - this.SalePrice) / this.RegularPrice;
                    return Convert.ToDecimal(number).ToString("#.#%");
                }
                else {
                    return "";
                }
            }
        }

        [NotMapped]
        public virtual string FormatedCurrentPrice
        {
            get
            {
                return Convert.ToDecimal(this.CurrentPrice).ToString("#,#đ");
            }
        }

        [NotMapped]
        public virtual string FormatedSalePrice
        {
            get
            {
                return Convert.ToDecimal(this.SalePrice).ToString("#,#đ");
            }
        }

        [NotMapped]
        public virtual string FormatedRegularPrice
        {
            get
            {
                return Convert.ToDecimal(this.RegularPrice).ToString("#,#đ");
            }
        }

        [NotMapped]
        public virtual List<MediaFile>  MediaFiles {
            get; set;        }

    }
}
