using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCVShared.Data
{
    public class ShopAttrib
    {
        public int Id { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "Tên gọi")]
        public string Name { get; set; }

        [Required, StringLength(128)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
    }

    public class AttribValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
    }

    public class ShopItemAttrib
    {
        public int Id { get; set; }

        [Display(Name = "Thuộc tính")]
        public int AttrId { get; set; }

        [Display(Name = "Vật phẩm")]
        public int ItemId { get; set; }

        [Display(Name = "Bắt buộc")]
        public bool IsRequired { get; set; }

        [Display(Name = "Giá trị")]
        public string Values { get; set; }

        [ForeignKey("AttrId")]
        public virtual ShopAttrib ShopAttrib { get; set; }

        [ForeignKey("ItemId")]
        public virtual ShopItem ShopItem { get; set; }

        [NotMapped]
        private List<AttribValue> valuesList;

        [NotMapped]
        [Display(Name = "Giá trị")]
        public virtual List<AttribValue> ValuesList
        {
            get
            {
                if (valuesList == null)
                    valuesList = Values == null ? new List<AttribValue>() : JsonConvert.DeserializeObject<List<AttribValue>>(Values);

                return valuesList;
            }
            set
            {
                valuesList = value;
                OnUpdateValues();
            }
        }

        public void OnUpdateValues()
        {
            Values = valuesList == null ? null : JsonConvert.SerializeObject(valuesList);
        }
    }
}
