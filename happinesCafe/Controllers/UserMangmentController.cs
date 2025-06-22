using happinesCafe.DATA;
using happinesCafe.Models;
using happinesCafe.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace happinesCafe.Controllers
{
    public class UserMangmentController : Controller
    {
        private readonly CaffeeSystemContext _context;

        public UserMangmentController(CaffeeSystemContext context)
        {
            _context = context;
        }
        //
        public IActionResult DisplayUsers()
        {
            IEnumerable<User> usersList = _context.Users.Include(c => c.IdRoleNavigation)
                .Include(c => c.StatusNavigation).ToList();
            return View(usersList);
        }
        //
        public IActionResult Show(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var user = _context.Users.Include(c => c.IdRoleNavigation)
                .Include(c => c.StatusNavigation).FirstOrDefault(p => p.IdUser == id);
            if (user == null || user.IdUser != id)
            {
                return NotFound();
            }
            return View(user);
        }
        ///
        public async Task<IActionResult> Orders(int id)
        {
            // Start with a queryable object
            IQueryable<Order> ordersQuery = _context.Orders
                                                .Include(o => o.IdUserNavigation)  // Use generated Navigation Property
                                                .Include(o => o.IdStateNavigation) // Use generated Navigation Property
                                                .OrderByDescending(o => o.OrderDate); // Default sort
                                                                                      // Execute query and project to ViewModel
            var orderListViewModels = await ordersQuery.Where(x => x.IdUserNavigation.IdUser == id)
                .Select(o => new OrderListViewModel
                {
                    IdOrder = o.IdOrder,
                    CustomerName = o.IdUserNavigation != null ? o.IdUserNavigation.NameUser : "N/A", // Handle potential null user
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalePrice, // Direct cast might work, otherwise (double)o.TotalePrice
                    OrderStatus = o.IdStateNavigation != null ? o.IdStateNavigation.NameState : "N/A" // Handle potential null state
                })
                .ToListAsync(); // Execute the query asynchronously


            return View(orderListViewModels);
        }


    }
}
