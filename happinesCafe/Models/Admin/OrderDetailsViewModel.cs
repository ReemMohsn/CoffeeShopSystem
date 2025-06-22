using happinesCafe.Models; // Your EF Models
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For validation/display attributes if needed

namespace happinesCafe.Models.Admin
{
    public class OrderDetailsViewModel
    {
        public int IdOrder { get; set; }
        public string CustomerName { get; set; } = string.Empty; // Initialize string properties
        // public string CustomerPhoneNumber { get; set; } = string.Empty; // Uncomment if used
        // public string CustomerAddress { get; set; } = string.Empty; // Uncomment if used
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; } // Match the type from Order model
        public string OrderStatus { get; set; } = string.Empty;
        public int CurrentStatusId { get; set; } // ID of the current status

        public List<OrderItemDetailsViewModel> OrderedItems { get; set; } = new List<OrderItemDetailsViewModel>();

        // For the status update dropdown
        public List<OrderState> AvailableStates { get; set; } = new List<OrderState>(); // Use OrderState model directly

        [Required] // Make sure a status is selected when submitting
        [Display(Name = "New Status")]
        public int NewStatusId { get; set; } // ID of the status to change to
    }

    // You should also have this ViewModel defined somewhere
    public class OrderItemDetailsViewModel
    {
        // We don't link directly to Product ID anymore, but through ProductSize ID
        // public int IdProduct { get; set; } // REMOVE or keep if needed for some other purpose

        public int ProductSizeId { get; set; } // The ID from the ProductsSize table
        public string ProductName { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty; // Add Size Name
        public int Quantity { get; set; }
        public double UnitPrice { get; set; } // Price comes from ProductsSize
        public double LineTotal { get; set; } // Calculated total for this item line
    }
}
