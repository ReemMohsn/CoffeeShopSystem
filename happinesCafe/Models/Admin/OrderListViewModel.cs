namespace happinesCafe.Models.Admin
{
    public class OrderListViewModel
    {
        public int IdOrder { get; set; }
        public string? CustomerName { get; set; } // Nullable in case user is deleted?
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; } // Use double or decimal
        public string? OrderStatus { get; set; } // Nullable if state is deleted?
    }
}
