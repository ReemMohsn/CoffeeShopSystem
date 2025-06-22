// ~/Controllers/ReviewManagementController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using happinesCafe.DATA;
using happinesCafe.Models;
using happinesCafe.Models.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace happinesCafe.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ReviewManagementController : Controller
    {
        private readonly CaffeeSystemContext _context;
        private const int ReviewtextTruncateLength = 50; // Max length for short review text

        public ReviewManagementController(CaffeeSystemContext context)
        {
            _context = context;
        }

        // GET: ReviewManagement
        public async Task<IActionResult> Index(string searchFilter = "") // Removed approvalFilter
        {
            ViewBag.SearchFilter = searchFilter;

            IQueryable<Review> reviewsQuery = _context.Reviews
                .Include(r => r.IdUserNavigation)
                .Include(r => r.IdProductNavigation)
                .OrderByDescending(r => r.ReviewDate); // Show newest first (consider handling null dates if applicable)

            // Apply Search Filter (Customer Name, Product Name, or Review Text)
            if (!string.IsNullOrEmpty(searchFilter))
            {
                reviewsQuery = reviewsQuery.Where(r =>
                    (r.IdUserNavigation != null && r.IdUserNavigation.NameUser.Contains(searchFilter)) ||
                    (r.IdProductNavigation != null && r.IdProductNavigation.NameProduct.Contains(searchFilter)) ||
                    (r.Reviewtext != null && r.Reviewtext.Contains(searchFilter)) // Use Reviewtext
                );
            }

            var reviewListViewModels = await reviewsQuery
                .Select(r => new ReviewListViewModel
                {
                    IdReview = r.IdReview,
                    CustomerName = r.IdUserNavigation != null ? r.IdUserNavigation.NameUser : "N/A",
                    ProductName = r.IdProductNavigation != null ? r.IdProductNavigation.NameProduct : "N/A",
                    Rating = r.Rating,
                    // Use Reviewtext and truncate
                    ShortReviewtext = r.Reviewtext != null && r.Reviewtext.Length > ReviewtextTruncateLength
                                   ? r.Reviewtext.Substring(0, ReviewtextTruncateLength) + "..."
                                   : r.Reviewtext ?? "", // Handle potential null Reviewtext
                    ReviewDate = r.ReviewDate // Assign nullable date
                })
                .ToListAsync();

            return View(reviewListViewModels);
        }

        // GET: ReviewManagement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.IdUserNavigation)
                .Include(r => r.IdProductNavigation)
                .FirstOrDefaultAsync(m => m.IdReview == id);

            if (review == null) return NotFound();

            var viewModel = new ReviewDetailsViewModel
            {
                IdReview = review.IdReview,
                CustomerName = review.IdUserNavigation?.NameUser ?? "N/A",
                ProductName = review.IdProductNavigation?.NameProduct ?? "N/A",
                Rating = review.Rating,
                Reviewtext = review.Reviewtext ?? "", // Use Reviewtext, handle null
                ReviewDate = review.ReviewDate // Assign nullable date
                // No IsApproved property needed
            };

            return View(viewModel);
        }

        // POST: ReviewManagement/ToggleApproval/5 <-- THIS ACTION IS REMOVED -->


        // GET: ReviewManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.IdUserNavigation)
                .Include(r => r.IdProductNavigation)
                .FirstOrDefaultAsync(m => m.IdReview == id);

            if (review == null) return NotFound();

            var viewModel = new ReviewDetailsViewModel // Reuse details viewmodel
            {
                IdReview = review.IdReview,
                CustomerName = review.IdUserNavigation?.NameUser ?? "N/A",
                ProductName = review.IdProductNavigation?.NameProduct ?? "N/A",
                Rating = review.Rating,
                Reviewtext = review.Reviewtext ?? "", // Use Reviewtext, handle null
                ReviewDate = review.ReviewDate
                // No IsApproved property needed
            };

            return View(viewModel); // Pass details to Delete confirmation view
        }

        // POST: ReviewManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                TempData["ErrorMessage"] = "Review not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log exception (ex)
                TempData["ErrorMessage"] = "An error occurred while deleting the review.";
                // Consider redirecting back to Delete view with error?
                // return View(MapToDetailsViewModel(review)); // Need a mapping function or recreate VM
            }

            return RedirectToAction(nameof(Index));
        }

        // Optional Helper method to map Review to ViewModel if needed in multiple places
        // private ReviewDetailsViewModel MapToDetailsViewModel(Review review) { ... }
    }
}
