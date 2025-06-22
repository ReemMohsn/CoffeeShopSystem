using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class ProductsSize
    {
        public ProductsSize()
        {
            Baskets = new HashSet<Basket>();
            OrderItems = new HashSet<OrderItem>();
        }

        public int IdSize { get; set; }
        public int IdProduct { get; set; }
        public double Price { get; set; }
        public int Id { get; set; }

        public virtual Product IdProductNavigation { get; set; } = null!;
        public virtual Size IdSizeNavigation { get; set; } = null!;
        public virtual ICollection<Basket> Baskets { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
