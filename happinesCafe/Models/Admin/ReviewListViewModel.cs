namespace happinesCafe.Models.Admin
{
    public class ReviewListViewModel
    {
        public int IdReview { get; set; }
        public string CustomerName { get; set; } = "N/A";
        public string ProductName { get; set; } = "N/A";
        public int Rating { get; set; }
        public string ShortReviewtext { get; set; } = string.Empty; // Renamed from ShortComment
        public DateTime? ReviewDate { get; set; } // Match Nullable DateTime from model
    }


    public class ReviewDetailsViewModel
    {
        public int IdReview { get; set; }
        public string CustomerName { get; set; } = "N/A";
        public string ProductName { get; set; } = "N/A";
        public int Rating { get; set; }
        public string Reviewtext { get; set; } = string.Empty; // Renamed from Comment
        public DateTime? ReviewDate { get; set; } // Match Nullable DateTime from model
    }
}
