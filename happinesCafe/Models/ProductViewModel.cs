namespace happinesCafe.Models
{
    public class ProductViewModel
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int IdCategory { get; set; }
        public string Picture { get; set; } = null!;
        public string? About { get; set; }
        public Dictionary<int, double> Prices { get; set; } // الأسعار حسب الحجم
        public double MinPrice { get; set; } // أقل سعر
        public double AverageRating { get; set; } // إضافة خاصية متوسط التقييم

    }
}
