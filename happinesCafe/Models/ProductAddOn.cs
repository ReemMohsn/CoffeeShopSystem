using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class ProductAddOn
    {
        public ProductAddOn()
        {
            BasketProductAddOns = new HashSet<BasketProductAddOn>();
            OrderItemsProductAddOns = new HashSet<OrderItemsProductAddOn>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int IdType { get; set; }
        public double Price { get; set; }

        public virtual ProductAddOnsTyp IdTypeNavigation { get; set; } = null!;
        public virtual ICollection<BasketProductAddOn> BasketProductAddOns { get; set; }
        public virtual ICollection<OrderItemsProductAddOn> OrderItemsProductAddOns { get; set; }
    }
}
