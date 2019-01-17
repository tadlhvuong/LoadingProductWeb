using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TCVShared.Data;

namespace TCVWeb.Areas.Admin.Models
{
    public class UpdateOrderModel
    {
        public int Id { get; set; }

        [Display(Name = "Trạng thái Đơn hàng")]
        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Trạng thái Thanh toán")]
        public PaymentStatus PaymentStatus { get; set; }
    }

    public class UpdateOrderItemModel
    {
        public int Id { get; set; }

        public string ItemName { get; set; }

        public double CurrentPrice { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Thành tiền")]
        public string SubTotal { get; set; }

    }
}
