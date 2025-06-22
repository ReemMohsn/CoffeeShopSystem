using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using happinesCafe.Models; // EF Models
using happinesCafe.Models.Admin; // ViewModels
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using happinesCafe.DATA;
// using Microsoft.AspNetCore.Authorization; // Keep commented as requested

namespace happinesCafe.Controllers
{
    //[Authorize(Roles = "Admin")] // Keep commented as requested
    public class OrderMangementController : Controller
    {
        private readonly CaffeeSystemContext _context;

        // Constants for status names (match DB insert data)
        private const string StatusCompleted = "Completed"; // Updated
        private const string StatusCancelled = "Cancelled";
        private const string StatusPending = "Pending";
        private const string StatusProcessing = "Processing";

        public OrderMangementController(CaffeeSystemContext context)
        {
            _context = context;
        }

        // GET: OrderMangement
        public async Task<IActionResult> Index(string? statusFilter = null, string? dateFilter = null, string? searchFilter = null)
        {
            // Start with a queryable object
            IQueryable<Order> ordersQuery = _context.Orders
                                                .Include(o => o.IdUserNavigation)  // Navigation to User
                                                .Include(o => o.IdStateNavigation) // Navigation to OrderState
                                                .OrderByDescending(o => o.OrderDate); // Default sort

            // Apply Status Filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                ordersQuery = ordersQuery.Where(o => o.IdStateNavigation != null && o.IdStateNavigation.NameState == statusFilter);
            }

            // Apply Date Filter
            if (DateTime.TryParse(dateFilter, out DateTime parsedDate))
            {
                // Filter for the specific day (order_date is 'date' type in DB)
                ordersQuery = ordersQuery.Where(o => o.OrderDate == parsedDate.Date);
            }

            // Apply Customer Name or Order ID Filter
            if (!string.IsNullOrEmpty(searchFilter))
            {
                bool isOrderId = int.TryParse(searchFilter, out int orderId);
                string searchLower = searchFilter.ToLower(); // For case-insensitive search on name

                ordersQuery = ordersQuery.Where(o =>
                    (o.IdUserNavigation != null && o.IdUserNavigation.NameUser.ToLower().Contains(searchLower)) ||
                    (isOrderId && o.IdOrder == orderId)
                );
            }

            // Execute query and project to ViewModel
            var orderListViewModels = await ordersQuery
                .Select(o => new OrderListViewModel
                {
                    IdOrder = o.IdOrder,
                    CustomerName = o.IdUserNavigation != null ? o.IdUserNavigation.NameUser : "N/A",
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalePrice, // double from DB maps to double in VM
                    OrderStatus = o.IdStateNavigation != null ? o.IdStateNavigation.NameState : "N/A"
                })
                .ToListAsync();

            // Pass filter values and options back to the view
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DateFilter = dateFilter;
            ViewBag.SearchFilter = searchFilter;
            // Use OrderStates DbSet and select NameState
            ViewBag.AvailableStatuses = await _context.OrderStates.Select(s => s.NameState).OrderBy(n => n).ToListAsync(); // Get List<string>

            return View(orderListViewModels);
        }

        // GET: OrderMangement/OrderDetails/5
        // OrderMangementController.cs

        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // --- Try original Include/ThenInclude approach again, but ensure it's correct ---
            // This is generally the cleaner way if relationships are correct.
            var order = await _context.Orders
                .Include(o => o.IdUserNavigation)     // User who placed the order
                .Include(o => o.IdStateNavigation)    // Current order state
                .Include(o => o.OrderItems)           // *** Load the OrderItems collection ***
                    .ThenInclude(oi => oi.IdProductSizeNavigation) // -> ProductsSize for each item
                        .ThenInclude(ps => ps.IdProductNavigation) // -> Product from ProductsSize
                .Include(o => o.OrderItems)           // Separate Include path needed for Size
                    .ThenInclude(oi => oi.IdProductSizeNavigation) // -> ProductsSize again
                        .ThenInclude(ps => ps.IdSizeNavigation)    // -> Size table from ProductsSize
                .FirstOrDefaultAsync(o => o.IdOrder == id); // Find the specific order


            if (order == null)
            {
                return NotFound(); // Order itself not found
            }

            // --- ADD SIMPLE LOGGING (For Debugging) ---
            Console.WriteLine($"---> Found Order ID: {order.IdOrder}");
            if (order.OrderItems != null)
            {
                Console.WriteLine($"---> Initial OrderItems Count: {order.OrderItems.Count}");
                foreach (var item in order.OrderItems)
                {
                    Console.WriteLine($"---> Item ID: {item.IdOrderItem}, ProductSize ID: {item.IdProductSize}, ProductSizeNav null? {item.IdProductSizeNavigation == null}");
                    if (item.IdProductSizeNavigation != null)
                    {
                        Console.WriteLine($"    ---> Product Nav null? {item.IdProductSizeNavigation.IdProductNavigation == null}, Size Nav null? {item.IdProductSizeNavigation.IdSizeNavigation == null}");
                    }
                }
            }
            else
            {
                Console.WriteLine("---> OrderItems collection is NULL.");
            }
            // -----------------------------------------


            // --- FIXED Declaration order ---
            // Declare ViewModel first
            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                IdOrder = order.IdOrder,
                CustomerName = order.IdUserNavigation?.NameUser ?? "N/A",
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalePrice,
                OrderStatus = order.IdStateNavigation?.NameState ?? "N/A",
                CurrentStatusId = order.IdState,
                AvailableStates = await _context.OrderStates.OrderBy(s => s.NameState).ToListAsync(),
                OrderedItems = new List<OrderItemDetailsViewModel>() // Initialize with empty list
            };

            // --- Process items ONLY IF they were loaded ---
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                Console.WriteLine("---> Projecting OrderItems to ViewModel...");
                orderDetailsViewModel.OrderedItems = order.OrderItems.Select(oi =>
                {
                    // Log inside the projection for detailed debugging
                    Console.WriteLine($"    -> Processing Item ID: {oi.IdOrderItem}");
                    var productName = oi.IdProductSizeNavigation?.IdProductNavigation?.NameProduct ?? "[Product Missing]";
                    var sizeName = oi.IdProductSizeNavigation?.IdSizeNavigation?.Name ?? "[Size Missing]";
                    var unitPrice = oi.IdProductSizeNavigation?.Price ?? 0;
                    Console.WriteLine($"       -> Product: {productName}, Size: {sizeName}, Price: {unitPrice}");

                    return new OrderItemDetailsViewModel
                    {
                        ProductSizeId = oi.IdProductSize,
                        ProductName = productName,
                        SizeName = sizeName,
                        Quantity = oi.Quantity,
                        UnitPrice = unitPrice,
                        LineTotal = oi.Quantity * unitPrice
                    };
                }).ToList(); // Convert selection to List
                Console.WriteLine($"---> Finished projection. ViewModel Items Count: {orderDetailsViewModel.OrderedItems.Count}");
            }
            else
            {
                Console.WriteLine($"---> Skipping item projection as OrderItems loaded count is {order.OrderItems?.Count ?? 0}.");
            }


            return View(orderDetailsViewModel);
        }




        // POST: OrderMangement/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Only bind the property coming from the form: NewStatusId
        public async Task<IActionResult> UpdateStatus(int id, [Bind("NewStatusId")] OrderDetailsViewModel viewModel)
        {
            // REMOVED the check: if (id != viewModel.IdOrder) ...

            var order = await _context.Orders
                                            .Include(o => o.IdStateNavigation)
                                            .FirstOrDefaultAsync(o => o.IdOrder == id); // Find using the route 'id'

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found."); // More specific NotFound
            }

            // Check if the target status ID exists
            var newStateExists = await _context.OrderStates.AnyAsync(s => s.IdState == viewModel.NewStatusId);
            if (!newStateExists)
            {
                TempData["ErrorMessage"] = "Invalid status selected.";
                return RedirectToAction("OrderDetails", new { id = id }); // Redirect back using the route 'id'
            }

            // Use constants for status names (Optional business logic)
            string currentStatusName = order.IdStateNavigation?.NameState ?? "";
            if (currentStatusName == StatusCancelled || currentStatusName == StatusCompleted)
            {
                TempData["ErrorMessage"] = $"Cannot update status. Order is already {currentStatusName}.";
                return RedirectToAction("OrderDetails", new { id = id });
            }

            // Update the foreign key using the validated ViewModel property
            order.IdState = viewModel.NewStatusId;

            try
            {
                _context.Update(order); // Or just SaveChangesAsync() if tracking is standard
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Could not update status due to a concurrency issue. Please refresh and try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status for ID {id}: {ex.ToString()}");
                TempData["ErrorMessage"] = "An error occurred while updating the status.";
            }

            return RedirectToAction("OrderDetails", new { id = id }); // Always use the route 'id' for redirect
        }



        // POST: OrderMangement/CancelOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id) // Only need the ID
        {
            var order = await _context.Orders
                                   .Include(o => o.IdStateNavigation) // Include current state for checking
                                   .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null)
            {
                return NotFound();
            }

            // Find the "Cancelled" status using the constant
            var cancelledState = await _context.OrderStates.FirstOrDefaultAsync(s => s.NameState == StatusCancelled);
            if (cancelledState == null)
            {
                TempData["ErrorMessage"] = $"'{StatusCancelled}' status not found in the database configuration.";
                return RedirectToAction("OrderDetails", new { id = id });
            }

            // Optional: Check if already cancelled or completed
            string currentStatusName = order.IdStateNavigation?.NameState ?? "";
            if (currentStatusName == StatusCancelled || currentStatusName == StatusCompleted)
            {
                TempData["ErrorMessage"] = $"Order is already {currentStatusName} and cannot be cancelled.";
                return RedirectToAction("OrderDetails", new { id = id });
            }

            // Update the foreign key
            order.IdState = cancelledState.IdState;

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception details (ex)
                Console.WriteLine($"Error cancelling order: {ex.ToString()}"); // Basic logging
                TempData["ErrorMessage"] = "An error occurred while cancelling the order.";
            }

            return RedirectToAction("OrderDetails", new { id = id });
        }
    }
}

﻿//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using happinesCafe.DATA;
//using happinesCafe.Models; // EF Models
//using happinesCafe.Models.Admin; // ViewModels
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//// using Microsoft.AspNetCore.Authorization; // Keep commented as requested

//namespace happinesCafe.Controllers
//{
//    //[Authorize(Roles = "Admin")] // Keep commented as requested
//    public class OrderMangementController : Controller
//    {
//        private readonly CaffeeSystemContext _context;

//        // Constants for status names (match DB insert data)
//        private const string StatusCompleted = "Completed"; // Updated
//        private const string StatusCancelled = "Cancelled";
//        private const string StatusPending = "Pending";
//        private const string StatusProcessing = "Processing";

//        public OrderMangementController(CaffeeSystemContext context)
//        {
//            _context = context;
//        }

//        // GET: OrderMangement
//        public async Task<IActionResult> Index(string? statusFilter = null, string? dateFilter = null, string? searchFilter = null)
//        {
//            // Start with a queryable object
//            IQueryable<Order> ordersQuery = _context.Orders
//                                                .Include(o => o.IdUserNavigation)  // Navigation to User
//                                                .Include(o => o.IdStateNavigation) // Navigation to OrderState
//                                                .OrderByDescending(o => o.OrderDate); // Default sort

//            if (!string.IsNullOrEmpty(statusFilter))
//            {
//                ordersQuery = ordersQuery.Where(o => o.IdStateNavigation != null && o.IdStateNavigation.NameState == statusFilter);
//            }

//            if (DateTime.TryParse(dateFilter, out DateTime parsedDate))
//            {
//                ordersQuery = ordersQuery.Where(o => o.OrderDate == parsedDate.Date);
//            }

//            if (!string.IsNullOrEmpty(searchFilter))
//            {
//                bool isOrderId = int.TryParse(searchFilter, out int orderId);
//                string searchLower = searchFilter.ToLower(); // For case-insensitive search on name

//                ordersQuery = ordersQuery.Where(o =>
//                    (o.IdUserNavigation != null && o.IdUserNavigation.NameUser.ToLower().Contains(searchLower)) ||
//                    (isOrderId && o.IdOrder == orderId)
//                );
//            }

//            var orderListViewModels = await ordersQuery
//                .Select(o => new OrderListViewModel
//                {
//                    IdOrder = o.IdOrder,
//                    CustomerName = o.IdUserNavigation != null ? o.IdUserNavigation.NameUser : "N/A",
//                    OrderDate = o.OrderDate,
//                    TotalPrice = o.TotalePrice, // double from DB maps to double in VM
//                    OrderStatus = o.IdStateNavigation != null ? o.IdStateNavigation.NameState : "N/A"
//                })
//                .ToListAsync();

//            // Pass filter values and options back to the view
//            ViewBag.StatusFilter = statusFilter;
//            ViewBag.DateFilter = dateFilter;
//            ViewBag.SearchFilter = searchFilter;
//            // Use OrderStates DbSet and select NameState
//            ViewBag.AvailableStatuses = await _context.OrderStates.Select(s => s.NameState).OrderBy(n => n).ToListAsync(); // Get List<string>

//            return View(orderListViewModels);
//        }

//        // GET: OrderMangement/OrderDetails/5
//        // OrderMangementController.cs

//        public async Task<IActionResult> OrderDetails(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var order = await _context.Orders
//                .Include(o => o.IdUserNavigation)     // User who placed the order
//                .Include(o => o.IdStateNavigation)    // Current order state
//                .Include(o => o.OrderItems)           // *** Load the OrderItems collection ***
//                    .ThenInclude(oi => oi.IdProductSizeNavigation) // -> ProductsSize for each item
//                        .ThenInclude(ps => ps.IdProductNavigation) // -> Product from ProductsSize
//                .Include(o => o.OrderItems)           // Separate Include path needed for Size
//                    .ThenInclude(oi => oi.IdProductSizeNavigation) // -> ProductsSize again
//                        .ThenInclude(ps => ps.IdSizeNavigation)    // -> Size table from ProductsSize
//                .FirstOrDefaultAsync(o => o.IdOrder == id); // Find the specific order


//            if (order == null)
//            {
//                return NotFound(); // Order itself not found
//            }

//            if (order.OrderItems != null)
//            {
//                foreach (var item in order.OrderItems)
//                {
//                    if (item.IdProductSizeNavigation != null)
//                    {

//                    }
//                }
//            }
//            else
//            {
//                Console.WriteLine("---> OrderItems collection is NULL.");
//            }
//            // -----------------------------------------


//            // --- FIXED Declaration order ---
//            // Declare ViewModel first
//            var orderDetailsViewModel = new OrderDetailsViewModel
//            {
//                IdOrder = order.IdOrder,
//                CustomerName = order.IdUserNavigation?.NameUser ?? "N/A",
//                OrderDate = order.OrderDate,
//                TotalPrice = order.TotalePrice,
//                OrderStatus = order.IdStateNavigation?.NameState ?? "N/A",
//                CurrentStatusId = order.IdState,
//                AvailableStates = await _context.OrderStates.OrderBy(s => s.NameState).ToListAsync(),
//                OrderedItems = new List<OrderItemDetailsViewModel>() // Initialize with empty list
//            };

//            // --- Process items ONLY IF they were loaded ---
//            if (order.OrderItems != null && order.OrderItems.Any())
//            {
//                Console.WriteLine("---> Projecting OrderItems to ViewModel...");
//                orderDetailsViewModel.OrderedItems = order.OrderItems.Select(oi =>
//                {
//                    // Log inside the projection for detailed debugging
//                    Console.WriteLine($"    -> Processing Item ID: {oi.IdOrderItem}");
//                    var productName = oi.IdProductSizeNavigation?.IdProductNavigation?.NameProduct ?? "[Product Missing]";
//                    var sizeName = oi.IdProductSizeNavigation?.IdSizeNavigation?.Name ?? "[Size Missing]";
//                    var unitPrice = oi.IdProductSizeNavigation?.Price ?? 0;
//                    Console.WriteLine($"       -> Product: {productName}, Size: {sizeName}, Price: {unitPrice}");

//                    return new OrderItemDetailsViewModel
//                    {
//                        ProductSizeId = oi.IdProductSize,
//                        ProductName = productName,
//                        SizeName = sizeName,
//                        Quantity = oi.Quantity,
//                        UnitPrice = unitPrice,
//                        LineTotal = oi.Quantity * unitPrice
//                    };
//                }).ToList(); // Convert selection to List
//                Console.WriteLine($"---> Finished projection. ViewModel Items Count: {orderDetailsViewModel.OrderedItems.Count}");
//            }
//            else
//            {
//                Console.WriteLine($"---> Skipping item projection as OrderItems loaded count is {order.OrderItems?.Count ?? 0}.");
//            }


//            return View(orderDetailsViewModel);
//        }




//        // POST: OrderMangement/UpdateStatus/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        // Only bind the property coming from the form: NewStatusId
//        public async Task<IActionResult> UpdateStatus(int id, [Bind("NewStatusId")] OrderDetailsViewModel viewModel)
//        {
//            // REMOVED the check: if (id != viewModel.IdOrder) ...

//            var order = await _context.Orders
//                                            .Include(o => o.IdStateNavigation)
//                                            .FirstOrDefaultAsync(o => o.IdOrder == id); // Find using the route 'id'

//            if (order == null)
//            {
//                return NotFound($"Order with ID {id} not found."); // More specific NotFound
//            }

//            // Check if the target status ID exists
//            var newStateExists = await _context.OrderStates.AnyAsync(s => s.IdState == viewModel.NewStatusId);
//            if (!newStateExists)
//            {
//                TempData["ErrorMessage"] = "Invalid status selected.";
//                return RedirectToAction("OrderDetails", new { id = id }); // Redirect back using the route 'id'
//            }

//            // Use constants for status names (Optional business logic)
//            string currentStatusName = order.IdStateNavigation?.NameState ?? "";
//            if (currentStatusName == StatusCancelled || currentStatusName == StatusCompleted)
//            {
//                TempData["ErrorMessage"] = $"Cannot update status. Order is already {currentStatusName}.";
//                return RedirectToAction("OrderDetails", new { id = id });
//            }

//            // Update the foreign key using the validated ViewModel property
//            order.IdState = viewModel.NewStatusId;

//            try
//            {
//                _context.Update(order); // Or just SaveChangesAsync() if tracking is standard
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Order status updated successfully.";
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                TempData["ErrorMessage"] = "Could not update status due to a concurrency issue. Please refresh and try again.";
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error updating order status for ID {id}: {ex.ToString()}");
//                TempData["ErrorMessage"] = "An error occurred while updating the status.";
//            }

//            return RedirectToAction("OrderDetails", new { id = id }); // Always use the route 'id' for redirect
//        }



//        // POST: OrderMangement/CancelOrder/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CancelOrder(int id) // Only need the ID
//        {
//            var order = await _context.Orders
//                                   .Include(o => o.IdStateNavigation) // Include current state for checking
//                                   .FirstOrDefaultAsync(o => o.IdOrder == id);
//            if (order == null)
//            {
//                return NotFound();
//            }

//            // Find the "Cancelled" status using the constant
//            var cancelledState = await _context.OrderStates.FirstOrDefaultAsync(s => s.NameState == StatusCancelled);
//            if (cancelledState == null)
//            {
//                TempData["ErrorMessage"] = $"'{StatusCancelled}' status not found in the database configuration.";
//                return RedirectToAction("OrderDetails", new { id = id });
//            }

//            // Optional: Check if already cancelled or completed
//            string currentStatusName = order.IdStateNavigation?.NameState ?? "";
//            if (currentStatusName == StatusCancelled || currentStatusName == StatusCompleted)
//            {
//                TempData["ErrorMessage"] = $"Order is already {currentStatusName} and cannot be cancelled.";
//                return RedirectToAction("OrderDetails", new { id = id });
//            }

//            // Update the foreign key
//            order.IdState = cancelledState.IdState;

//            try
//            {
//                _context.Update(order);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Order cancelled successfully.";
//            }
//            catch (Exception ex)
//            {
//                // Log the exception details (ex)
//                Console.WriteLine($"Error cancelling order: {ex.ToString()}"); // Basic logging
//                TempData["ErrorMessage"] = "An error occurred while cancelling the order.";
//            }

//            return RedirectToAction("OrderDetails", new { id = id });
//        }
//    }
//}
