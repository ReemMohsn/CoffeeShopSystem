namespace happinesCafe.Models
{
    public class ReviewViewModel
    {
        public string UserName { get; set; }
        public string UserImage { get; set; } // مسار صورة المستخدم
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDate { get; set; }

        // دالة مساعدة لعرض النجوم
        public string GetStarRating()
        {
            return string.Join("", Enumerable.Repeat("<img src='/imges/star2.png' class='starr'>", Rating));
        }
    }
}
