// ~/Controllers/ControlPanelController.cs - CHECK NAMESPACE
using happinesCafe.DATA;
using happinesCafe.Models;
using happinesCafe.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace happinesCafe.Controllers
{
    //[Authorize(Roles = "Admin")] // Keep commented as per your request
    public class ControlPanelController : Controller
    {
        private readonly CaffeeSystemContext _context;

        public ControlPanelController(CaffeeSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1); // Use '< startOfNextMonth' for comparisons
            var recentDateThreshold = today.AddDays(-7); // For recent products


            try
            {
                // Total Sales Today (Using Date property for comparison if OrderDate includes time)
                // Order.TotalePrice is double, ViewModel.TotalSalesToday is decimal. Explicit cast needed.
                viewModel.TotalSalesToday = (decimal?)await _context.Orders
                    .Where(o => o.OrderDate >= today && o.OrderDate < today.AddDays(1)) // Compare date range
                    .SumAsync(o => (double?)o.TotalePrice) ?? 0m; // Cast to nullable double for SumAsync, then to decimal


                // Total Sales This Month
                viewModel.TotalSalesThisMonth = (decimal?)await _context.Orders
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate < startOfNextMonth) // Use range
                    .SumAsync(o => (double?)o.TotalePrice) ?? 0m;


                // New Orders Today Count
                viewModel.NewOrdersTodayCount = await _context.Orders
                    .CountAsync(o => o.OrderDate >= today && o.OrderDate < today.AddDays(1)); // Use range


                // Recently Added Products Count (last 7 days)
                // Assumes product.CreatedDate does not include time (Type = date)
                viewModel.RecentProductsCount = await _context.Products
                    .CountAsync(p => p.CreatedDate >= recentDateThreshold);


                // Active Users Count
                // Get the ID for the 'Active' status from the UserStatus table
                var activeStatus = await _context.UserStatuses
                                    .FirstOrDefaultAsync(us => us.Name == "Active"); // Find 'Active'
                if (activeStatus != null)
                {
                    viewModel.ActiveUsersCount = await _context.Users
                        .CountAsync(u => u.Status == activeStatus.Id); // Compare FK ID
                }
                else
                {
                    viewModel.ActiveUsersCount = 0; // Handle if 'Active' status doesn't exist
                    ViewBag.WarningMessage = "Could not find 'Active' user status in the database.";
                }


                // Problematic Orders Count (Cancelled or Pending)
                // IMPORTANT: Adjust "Cancelled" and "Pending" if your state names differ
                string[] problematicStateNames = { "Cancelled", "Pending" };
                var problematicStateIds = await _context.OrderStates // Use OrderState model
                    .Where(os => problematicStateNames.Contains(os.NameState)) // Filter by NameState
                    .Select(os => os.IdState) // Get the IDs
                    .ToListAsync();

                viewModel.ProblematicOrdersCount = await _context.Orders
                    .CountAsync(o => problematicStateIds.Contains(o.IdState)); // Compare Order's IdState FK

            }
            catch (Exception ex)
            {
                // Log the error using a proper logging framework (e.g., ILogger)
                Console.WriteLine($"Error retrieving dashboard statistics: {ex.ToString()}"); // Basic console logging
                ViewBag.ErrorMessage = "An error occurred while retrieving dashboard statistics. Please check the logs.";
                // Avoid showing ex.Message directly to the user in production
            }

            return View(viewModel);
        }
    }
}
