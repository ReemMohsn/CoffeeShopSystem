using happinesCafe.DATA;
using happinesCafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using Microsoft.AspNetCore.Antiforgery; // Required for IAntiforgery

namespace happinesCafe.Controllers
{


    public class FavoritController : Controller
    {
        private readonly CaffeeSystemContext _db;
        // Inject Antiforgery service if needed elsewhere, but not strictly required for validation attribute
        // private readonly IAntiforgery _antiforgery;

        public FavoritController(CaffeeSystemContext context/*, IAntiforgery antiforgery*/)
        {
            _db = context;
            // _antiforgery = antiforgery;
        }

        // --- Index Action (remains the same) ---
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                ViewBag.ErrorMessage = "You must be logged in to view favorites.";
                return View(new List<FavoriteViewModel>());
            }

            var favoriteItems = await _db.Favorites
                .Where(f => f.IdUser == userId.Value)
                .Include(f => f.IdProductNavigation)
                .Select(f => new FavoriteViewModel
                {
                    ProductId = f.IdProduct,
                    ProductName = f.IdProductNavigation != null ? f.IdProductNavigation.NameProduct : "Product Not Found",
                    ProductPictureUrl = f.IdProductNavigation != null ? f.IdProductNavigation.Picture : null,
                    CategoryId = f.IdProductNavigation != null ? f.IdProductNavigation.IdCategory : 0,
                    ProductPictureClass = (f.IdProductNavigation != null && (f.IdProductNavigation.IdCategory == 1 || f.IdProductNavigation.IdCategory == 2)) ? "img-pro" :
                                          (f.IdProductNavigation != null && f.IdProductNavigation.IdCategory == 4) ? "img-pro-coffee" :
                                          "img-pro"
                })
                .ToListAsync();

            return View(favoriteItems);
        }

        // --- ToggleFavorite (MODIFIED to accept JSON and Validate Token) ---
        [HttpPost]
        [ValidateAntiForgeryToken] // Keep token validation - Token MUST be sent in request header
        // *** Changed back to [FromBody] to accept JSON consistently ***
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteTogglePayload payload)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "You must log in first.", redirectToLogin = true });
            }

            // *** Get productId from the payload ***
            var productId = payload.ProductId;

            try
            {
                var productExists = await _db.Products.AnyAsync(p => p.IdProduct == productId);
                if (!productExists)
                {
                    return NotFound(new { success = false, message = "Product not found." });
                }

                var userExists = await _db.Users.AnyAsync(u => u.IdUser == userId.Value);
                if (!userExists)
                {
                    return BadRequest(new { success = false, message = "User identifier invalid." });
                }

                var existingFavorite = await _db.Favorites
                    .FirstOrDefaultAsync(f => f.IdUser == userId.Value && f.IdProduct == productId);

                bool isNowFavorite;
                string message;

                if (existingFavorite != null)
                {
                    _db.Favorites.Remove(existingFavorite);
                    message = "Removed from favorites.";
                    isNowFavorite = false;
                }
                else
                {
                    var newFavorite = new Favorite { IdUser = userId.Value, IdProduct = productId, AddDate = DateTime.Now };
                    _db.Favorites.Add(newFavorite);
                    message = "Added to favorites.";
                    isNowFavorite = true;
                }

                await _db.SaveChangesAsync();
                return Ok(new { success = true, isFavorite = isNowFavorite, message = message });

            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error in ToggleFavorite: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(500, new { success = false, message = "A database error occurred while updating favorites." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleFavorite: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while updating favorites." });
            }
        }


        // --- CheckFavorite (Remains the same) ---
        [HttpGet]
        public async Task<IActionResult> CheckFavorite(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { isFavorite = false });
            }
            bool isFavorite = await _db.Favorites.AnyAsync(f => f.IdUser == userId.Value && f.IdProduct == productId);
            return Json(new { isFavorite });
        }

        // --- RedirectBasedOnType (Remains the same, not used by AJAX toggle) ---
        private IActionResult RedirectBasedOnType(int type, int productId)
        {
            return type switch
            {
                4 => RedirectToAction("ProductDetails", "Products", new { id = productId }),
                1 or 2 => RedirectToAction("ProductDetailsDrinks", "Products", new { id = productId }),
                3 => RedirectToAction("ProductDetailsSweets", "Products", new { id = productId }),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
