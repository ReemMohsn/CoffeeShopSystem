using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class Size
    {
        public Size()
        {
            ProductsSizes = new HashSet<ProductsSize>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<ProductsSize> ProductsSizes { get; set; }
    }
}
