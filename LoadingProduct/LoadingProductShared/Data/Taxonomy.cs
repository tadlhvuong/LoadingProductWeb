using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCVShared.Data
{
    public class Taxonomy
    {
        public int Id { get; set; }

        [Display(Name = "Cấp trên")]
        public int? ParentId { get; set; }

        [Display(Name = "Phân kiểu")]
        public TaxoType Type { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "Tên gọi")]
        public string Name { get; set; }

        [ForeignKey("ParentId")]
        public virtual Taxonomy Parent { get; set; }

        public virtual ICollection<Taxonomy> Children { get; set; }

        [NotMapped]
        public virtual string ParentName
        {
            get
            {
                if (ParentId == null)
                    return "NULL";
                Parent = new Taxonomy();
                return Parent.Name;
            }
        }
    }
}
