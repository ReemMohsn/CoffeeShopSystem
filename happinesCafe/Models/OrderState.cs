using System;
using System.Collections.Generic;

namespace happinesCafe.Models
{
    public partial class OrderState
    {
        public OrderState()
        {
            Orders = new HashSet<Order>();
        }

        public int IdState { get; set; }
        public string NameState { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
