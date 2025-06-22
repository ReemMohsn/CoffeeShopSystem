using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class Basket
    {
        public Basket()
        {
            BasketProductAddOns = new HashSet<BasketProductAddOn>();
        }

        public int IdBasket { get; set; }
        public int IdUser { get; set; }
        public int IdProductSize { get; set; }
        public int QuantityProduct { get; set; }
        public DateTime AddedDate { get; set; }
        public double TotalPrice { get; set; }

        public virtual ProductsSize IdProductSizeNavigation { get; set; } = null!;
        public virtual User IdUserNavigation { get; set; } = null!;
        public virtual ICollection<BasketProductAddOn> BasketProductAddOns { get; set; }
    }
}
