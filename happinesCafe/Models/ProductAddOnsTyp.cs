using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class ProductAddOnsTyp
    {
        public ProductAddOnsTyp()
        {
            ProductAddOns = new HashSet<ProductAddOn>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<ProductAddOn> ProductAddOns { get; set; }
    }
}
