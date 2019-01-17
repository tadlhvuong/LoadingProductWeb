using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCVShared.Data
{
    [Table("ShopWishItems")]
    public class WishItem
    {
        public int Id { get; set; }

        public int WishId { get; set; }

        public int ItemId { get; set; }

        [ForeignKey("WishId")]
        public virtual ShopWish ShopWish { get; set; }

        [ForeignKey("ItemId")]
        public virtual ShopItem ShopItem { get; set; }
    }
    public class ShopWish
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [Display(Name = "Sản phẩm")]
        public virtual ICollection<WishItem> Items { get; set; }
    }
}
