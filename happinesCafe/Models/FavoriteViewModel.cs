namespace happinesCafe.Models
{
    public class FavoriteViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductPictureUrl { get; set; }
        public string ProductPictureClass { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }

    // --- DTO for receiving JSON data from AJAX (needed again) ---
    public class FavoriteTogglePayload
    {
        public int ProductId { get; set; }
    }
}