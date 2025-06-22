namespace happinesCafe.Models.Admin
{
    public class DashboardViewModel
    {
        public decimal TotalSalesToday { get; set; }
        public decimal TotalSalesThisMonth { get; set; }
        public int NewOrdersTodayCount { get; set; }
        public int RecentProductsCount { get; set; } // e.g., last 7 days
        public int ActiveUsersCount { get; set; }
        public int ProblematicOrdersCount { get; set; } // Canceled or Delayed
        public DateTime StatisticsDate { get; set; } = DateTime.Now; // To show when stats were generated
    }
}
