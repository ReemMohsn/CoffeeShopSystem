using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using happinesCafe.DATA;
using happinesCafe.Models;
using System.Security.Claims;

namespace happinesCafe.Controllers
{
    public class BuyingController : Controller
    {
        private readonly CaffeeSystemContext _db;

        public BuyingController(CaffeeSystemContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Cart()
        {
            // التحقق من تسجيل دخول المستخدم
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "sign in first to see your cart";
                return View(new List<Basket>());
            }

            var cartItems = await _db.Baskets
                .Include(b => b.IdProductSizeNavigation)
                .ThenInclude(ps => ps.IdProductNavigation)
                .Include(b => b.IdProductSizeNavigation)
                .ThenInclude(ps => ps.IdSizeNavigation)
                .Include(b => b.BasketProductAddOns)
                .ThenInclude(a => a.IdProductAddOnsNavigation)
                .Where(b => b.IdUser == userId.Value) // استخدام .Value بدلاً من Parse
                .ToListAsync();

            return View(cartItems);
        }

        public async Task<IActionResult> UpdateQuantity(int basketId,string plusOrminus)
        {
            // البحث عن عنصر السلة مع متعلقاته (الإضافات)
            var basketItem = await _db.Baskets
                .Include(b => b.BasketProductAddOns)
                .FirstOrDefaultAsync(b => b.IdBasket == basketId);
            
            if (basketItem != null)
            {
                if(plusOrminus== "plus")
                {
                    basketItem.QuantityProduct++;
                    _db.Update(basketItem);
                }
                else
                {
                    basketItem.QuantityProduct--;
                    _db.Update(basketItem);
                    if (basketItem.QuantityProduct==0)
                    {
                        if (basketItem.BasketProductAddOns.Any())
                        {
                            _db.BasketProductAddOns.RemoveRange(basketItem.BasketProductAddOns);
                        }
                        _db.Baskets.Remove(basketItem);
                    }
                }
                await _db.SaveChangesAsync();
                return RedirectToAction("Cart");
            }
            return NotFound();
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int basketId)
        {
            try
            {
                // البحث عن عنصر السلة مع متعلقاته (الإضافات)
                var basketItem = await _db.Baskets
                    .Include(b => b.BasketProductAddOns)
                    .FirstOrDefaultAsync(b => b.IdBasket == basketId);

                if (basketItem == null)
                {
                    return NotFound();
                }

                // حذف جميع الإضافات المرتبطة بهذا العنصر
                if (basketItem.BasketProductAddOns.Any())
                {
                    _db.BasketProductAddOns.RemoveRange(basketItem.BasketProductAddOns);
                }

                // حذف عنصر السلة نفسه
                _db.Baskets.Remove(basketItem);

                await _db.SaveChangesAsync();
                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while trying to delete the item from the cart.");
            }
        }

        [HttpGet]
        //public IActionResult CheckOut(double totalPrice)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "You must log in to check out.";
        //        return RedirectToAction("Cart");
        //    }

        //    var user = _db.Users.Find(userId);

        //    // التحقق من وجود المستخدم
        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "user not found";
        //        return RedirectToAction("Cart");
        //    }

        //    ViewBag.totalprice = totalPrice;
        //    return View(user);
        //}

        //public IActionResult CheckOut(double totalPrice)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "You must log in to check out.";
        //        return RedirectToAction("Cart");
        //    }

        //    var user = _db.Users.Find(userId);

        //    // التحقق من وجود المستخدم
        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "user not found";
        //        return RedirectToAction("Cart");
        //    }

        //    var order = new Order()
        //    {
        //        IdUser = user.IdUser,
        //        TotalePrice = totalPrice,
        //        IdState = 1,
        //        OrderDate = DateTime.Now,
        //        NewOrNot = true

        //    };

        //    ViewBag.user=user;
        //    return View(order);
        //}

        //[HttpPost]
        //public async Task<IActionResult> CheckOut(Order ord)
        //{
        //    //ord.IdUserNavigation = await _db.Users.FindAsync(ord.IdUser);
        //    //ord.IdStateNavigation = await _db.OrderStates.FindAsync(ord.IdState);

        //    //ModelState.Clear();
        //    //TryValidateModel(ord);

        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "You must log in to complete the order";
        //        return RedirectToAction("Cart");
        //    }
        //    var user = _db.Users.Find(userId);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _db.Orders.Add(ord);
        //            await _db.SaveChangesAsync();

        //            var cartItems = await _db.Baskets
        //                .Where(b => b.IdUser == userId)
        //                .Include(b => b.IdProductSizeNavigation)
        //                .Include(b => b.BasketProductAddOns)
        //                .ToListAsync();

        //            // 3. إضافة عناصر الطلب
        //            foreach (var item in cartItems)
        //            {
        //                var orderItem = new OrderItem
        //                {
        //                    IdOrder = ord.IdOrder,
        //                    IdProductSize = item.IdProductSize,
        //                    Quantity = item.QuantityProduct,
        //                    TotalPrice = item.TotalPrice * item.QuantityProduct
        //                };

        //                _db.OrderItems.Add(orderItem);
        //                await _db.SaveChangesAsync(); 

        //                // 4. إضافة الإضافات إذا وجدت
        //                if (item.BasketProductAddOns.Any())
        //                {
        //                    foreach (var addon in item.BasketProductAddOns)
        //                    {
        //                        _db.OrderItemsProductAddOns.Add(new OrderItemsProductAddOn
        //                        {
        //                            IdOrderItem = orderItem.IdOrderItem,
        //                            IdProductAddOns = addon.IdProductAddOns
        //                        });
        //                    }
        //                }
        //            }

        //            // . تفريغ السلة
        //            foreach (var item in cartItems)
        //            {
        //                // حذف جميع الإضافات المرتبطة أولاً
        //                if (item.BasketProductAddOns.Any())
        //                {
        //                    _db.BasketProductAddOns.RemoveRange(item.BasketProductAddOns);
        //                    await _db.SaveChangesAsync();
        //                }

        //                // ثم حذف عنصر السلة نفسه
        //                _db.Baskets.Remove(item);
        //                await _db.SaveChangesAsync();
        //            }

        //            // 6. تأكيد كل العمليات
        //            await _db.SaveChangesAsync();
        //            TempData["SuccessMessage"] = "The order was sent successfully";
        //            return RedirectToAction("Index", "Home");
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] = "An error occurred while processing the order";
        //            return View(user);
        //        }
        //    }

        //    ViewBag.user = user;
        //    return View(ord);

        //}
        [HttpPost]
        public async Task<IActionResult> CheckOut(string address, string city, string phone, [FromForm(Name = "totalPrice")] double orderTotal)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must log in to complete the order";
                return RedirectToAction("Cart");
            }
            var user = _db.Users.Find(userId);


            try
            {
                // 1. إنشاء الطلب الأساسي
                var order = new Order
                {
                    IdUser = user.IdUser,
                    ShippingAddress = address,
                    City = city,
                    ContactPhone = phone,
                    IdState = 1,
                    OrderDate = DateTime.Now,
                    TotalePrice = orderTotal,
                    NewOrNot = true
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                var cartItems = await _db.Baskets
                    .Where(b => b.IdUser == userId)
                    .Include(b => b.IdProductSizeNavigation)
                    .Include(b => b.BasketProductAddOns)
                    .ToListAsync();

                // 3. إضافة عناصر الطلب
                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        IdOrder = order.IdOrder,
                        IdProductSize = item.IdProductSize,
                        Quantity = item.QuantityProduct,
                        TotalPrice = item.TotalPrice * item.QuantityProduct
                    };

                    _db.OrderItems.Add(orderItem);
                    await _db.SaveChangesAsync(); // حفظ عنصر الطلب للحصول على IdOrderItem

                    // 4. إضافة الإضافات إذا وجدت
                    if (item.BasketProductAddOns.Any())
                    {
                        foreach (var addon in item.BasketProductAddOns)
                        {
                            _db.OrderItemsProductAddOns.Add(new OrderItemsProductAddOn
                            {
                                IdOrderItem = orderItem.IdOrderItem,
                                IdProductAddOns = addon.IdProductAddOns
                            });
                        }
                    }
                }

                // 5. تفريغ السلة - الحل الموصى به
                foreach (var item in cartItems)
                {
                    // حذف جميع الإضافات المرتبطة أولاً
                    if (item.BasketProductAddOns.Any())
                    {
                        _db.BasketProductAddOns.RemoveRange(item.BasketProductAddOns);
                        await _db.SaveChangesAsync();
                    }

                    // ثم حذف عنصر السلة نفسه
                    _db.Baskets.Remove(item);
                    await _db.SaveChangesAsync();
                }

                // 6. تأكيد كل العمليات
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "The order was sent successfully";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the order";
                return View(user);
            }
        }

        public async Task<IActionResult> DisPlayOrders(string? statusFilter = null, string? dateFilter = null, string? searchFilter = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "يجب تسجيل الدخول لإتمام الطلب";
                return RedirectToAction("Cart");
            }
            IQueryable<Order> ordersQuery = _db.Orders
                                                .Include(o => o.IdStateNavigation) // Navigation to OrderState
                                                .Where(o=>o.IdUser== userId)
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

           
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DateFilter = dateFilter;
            ViewBag.SearchFilter = searchFilter;
            ViewBag.AvailableStatuses = await _db.OrderStates.Select(s => s.NameState).OrderBy(n => n).ToListAsync(); // Get List<string>

            return View(ordersQuery);
        }


        public async Task<IActionResult> OrderDetails(int idorder)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must log in to see your cart..";
                return View(new List<Basket>()); 
            }

            var orderItems = await _db.OrderItems
                 .Where(b => b.IdOrder == idorder)
                .Include(b => b.IdProductSizeNavigation)
                .ThenInclude(ps => ps.IdProductNavigation)
                .Include(b => b.IdProductSizeNavigation)
                .ThenInclude(ps => ps.IdSizeNavigation)
                .Include(b => b.OrderItemsProductAddOns)
                .ThenInclude(a => a.IdProductAddOnsNavigation)
                .ToListAsync();
            return View(orderItems);
        }


        public async Task<IActionResult> CancelOrder(int idorder) 
        {
            var order = await _db.Orders
                                   .Include(o => o.IdStateNavigation) 
                                   .FirstOrDefaultAsync(o => o.IdOrder == idorder);
            if (order == null)
            {
                return NotFound();
            }

            
            if (order.IdState==3)
            {
                TempData["ErrorMessage"] = $"Order is already completed and cannot be cancelled.";
                return RedirectToAction("OrderDetails", new { idorder = idorder });
            }
            if (order.IdState == 4)
            {
                TempData["ErrorMessage"] = $"Order is already cancelled.";
                return RedirectToAction("OrderDetails", new { idorder = idorder });
            }
            try
            {
                order.IdState = 4;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception details (ex)
                Console.WriteLine($"Error cancelling order: {ex.ToString()}"); // Basic logging
                TempData["ErrorMessage"] = "An error occurred while cancelling the order.";
            }

            return RedirectToAction("OrderDetails", new { idorder = idorder });
        }
    }
}
