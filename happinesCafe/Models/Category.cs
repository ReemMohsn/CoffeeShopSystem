using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int IdCategory { get; set; }
        public string NameCategory { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
