using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class User
    {
        public User()
        {
            Baskets = new HashSet<Basket>();
            Favorites = new HashSet<Favorite>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
        }

        public int IdUser { get; set; }
        public string NameUser { get; set; } = null!;
        public string EmailUser { get; set; } = null!;
        public string PasswordUser { get; set; } = null!;
        public int IdRole { get; set; }
        public DateTime CreatdDate { get; set; }
        public int Status { get; set; }
        public string? PictuerUser { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        public virtual Role IdRoleNavigation { get; set; } = null!;
        public virtual UserStatus StatusNavigation { get; set; } = null!;
        public virtual ICollection<Basket> Baskets { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
