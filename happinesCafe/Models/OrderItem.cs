using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class OrderItem
    {
        public OrderItem()
        {
            OrderItemsProductAddOns = new HashSet<OrderItemsProductAddOn>();
        }

        public int IdOrderItem { get; set; }
        public int IdOrder { get; set; }
        public int Quantity { get; set; }
        public int IdProductSize { get; set; }
        public double TotalPrice { get; set; }

        public virtual Order IdOrderNavigation { get; set; } = null!;
        public virtual ProductsSize IdProductSizeNavigation { get; set; } = null!;
        public virtual ICollection<OrderItemsProductAddOn> OrderItemsProductAddOns { get; set; }
    }
}
