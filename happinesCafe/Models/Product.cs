using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class Product
    {
        public Product()
        {
            Favorites = new HashSet<Favorite>();
            ProductsSizes = new HashSet<ProductsSize>();
            Reviews = new HashSet<Review>();
        }

        public int IdProduct { get; set; }
        public string NameProduct { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int IdCategory { get; set; }
        public string Picture { get; set; } = null!;
        public string? About { get; set; }

        public virtual Category IdCategoryNavigation { get; set; } = null!;
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<ProductsSize> ProductsSizes { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
