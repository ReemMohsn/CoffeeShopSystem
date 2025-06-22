using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class BasketProductAddOn
    {
        public int Id { get; set; }
        public int IdBasket { get; set; }
        public int IdProductAddOns { get; set; }

        public virtual Basket IdBasketNavigation { get; set; } = null!;
        public virtual ProductAddOn IdProductAddOnsNavigation { get; set; } = null!;
    }
}
