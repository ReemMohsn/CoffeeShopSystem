using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int IdOrder { get; set; }
        public int IdUser { get; set; }
        public int IdState { get; set; }
        public double TotalePrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool? NewOrNot { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;


        public virtual OrderState IdStateNavigation { get; set; } = null!;
        public virtual User IdUserNavigation { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
