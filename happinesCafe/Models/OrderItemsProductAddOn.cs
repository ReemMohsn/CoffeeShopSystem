using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class OrderItemsProductAddOn
    {
        public int Id { get; set; }
        public int IdProductAddOns { get; set; }
        public int IdOrderItem { get; set; }

        public virtual OrderItem IdOrderItemNavigation { get; set; } = null!;
        public virtual ProductAddOn IdProductAddOnsNavigation { get; set; } = null!;
    }
}
