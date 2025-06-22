using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class Favorite
    {
        public int IdFavorite { get; set; }
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public DateTime AddDate { get; set; }

        public virtual Product IdProductNavigation { get; set; } = null!;
        public virtual User IdUserNavigation { get; set; } = null!;
    }
}
