using Microsoft.AspNetCore.Mvc;
using happinesCafe.DATA;
using happinesCafe.Models;
using Microsoft.EntityFrameworkCore;

namespace happinesCafe.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CaffeeSystemContext _db;

        public ProductsController(CaffeeSystemContext context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartSimple(int productId)
        {
            // 1. التحقق من تسجيل دخول المستخدم
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    error = "You must log in first to add products to your cart"
                });
            }

            try
            {
                // 2. الحصول على أصغر حجم للمنتج (الحجم الافتراضي)
                var defaultSize = await _db.ProductsSizes
                    .Where(ps => ps.IdProduct == productId)
                    .Include(b => b.IdProductNavigation)
                    .OrderBy(ps => ps.IdSize)
                    .FirstOrDefaultAsync();

                if (defaultSize == null)
                {
                    return Json(new
                    {
                        success = false,
                        error = "No sizes available for this product."
                    });
                }

                // 3. التحقق من وجود المنتج في السلة
                var existingItem = await _db.Baskets
                    .FirstOrDefaultAsync(b => b.IdUser == userId && b.IdProductSize == defaultSize.Id);

                if (existingItem != null)
                {
                    existingItem.QuantityProduct += 1;
                    _db.Baskets.Update(existingItem);
                    await _db.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = $"تم تحديث كمية المنتج في السلة ({existingItem.QuantityProduct} قطعة)."
                    });
                }
                else
                {
                    // إضافة عنصر جديد إلى السلة
                    var newItem = new Basket
                    {
                        IdUser = userId.Value,
                        IdProductSize = defaultSize.Id,
                        QuantityProduct = 1,
                        AddedDate = DateTime.Now,
                        TotalPrice = defaultSize.Price
                    };
                    _db.Baskets.Add(newItem);

                    await _db.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "  Product successfully added to cart"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = "  An error occurred while adding the product to your cart."
                });
            }
        }

        [HttpPost] 
        public async Task<IActionResult> AddToBasket(int productId, int? sizeId )
        {
            // التحقق من وجود حجم مختار
            if (!sizeId.HasValue || sizeId.Value == 0)
            {
                TempData["ErrorMessage"] = "You must select the product size before adding to the cart";
                return RedirectToAction("ProductDetails", new { id = productId });
            }

            // 1. التحقق من تسجيل دخول المستخدم
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                
                TempData["ErrorMessage"] = "You must log in first to add products to the cart.";

                return RedirectToAction("ProductDetails", new { id = productId });
            }


            try
            {
                // 2. البحث عن ProductSize المطابق للحصول على المفتاح الأساسي والسعر
                var productSize = await _db.ProductsSizes
                                           .FirstOrDefaultAsync(ps => ps.IdProduct == productId && ps.IdSize == sizeId);

                if (productSize == null)
                {
                    TempData["ErrorMessage"] = "The selected size is not available for this product";
                    return RedirectToAction("ProductDetails", new { id = productId }); 
                }

                // 3. التحقق إذا كان المنتج موجوداً بالسلة وتحديث الكمية أو الإضافة
                var existingBasketItem = await _db.Baskets
                                                .FirstOrDefaultAsync(b => b.IdUser == userId.Value && b.IdProductSize == productSize.Id);

                if (existingBasketItem != null)
                {
                    existingBasketItem.QuantityProduct += 1; 
                    //existingBasketItem.TotalPrice = existingBasketItem.QuantityProduct * productSize.Price;
                    _db.Baskets.Update(existingBasketItem);
                    TempData["SuccessMessage"] = $"تم تحديث كمية المنتج في السلة بنجاح ({existingBasketItem.QuantityProduct} قطعة).";
                }
                else
                {
                    // 4. إنشاء عنصر سلة جديد
                    var basketItem = new Basket
                    {
                        IdUser = userId.Value,
                        IdProductSize = productSize.Id,
                        QuantityProduct = 1, 
                        AddedDate = DateTime.Now,
                        TotalPrice =  productSize.Price 
                    };
                    _db.Baskets.Add(basketItem);
                    TempData["SuccessMessage"] = "product add to cart success.";
                }

                // 5. حفظ التغييرات في قاعدة البيانات
                await _db.SaveChangesAsync();

                return RedirectToAction("ProductDetails", new { id = productId });

                           }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء غير المتوقعة
                TempData["ErrorMessage"] = "error aqured when add the product to cart ,try agin later";
                return RedirectToAction("ProductDetails", new { id = productId }); 
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddDrinkesToBasket(int productId, int? sizeId, int? Suger, int? Milk)
        {
            // التحقق من وجود حجم مختار (إلزامي)
            if (!sizeId.HasValue || sizeId.Value == 0)
            {
                TempData["ErrorMessage"] = "select the size of product befor add to cart";
                return RedirectToAction("ProductDetailsDrinks", new { id = productId });
            }

            // التحقق من تسجيل دخول المستخدم
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "must sign in first to add products to cart";
                return RedirectToAction("ProductDetailsDrinks", new { id = productId });
            }

            try
            {
                // البحث عن ProductSize المطابق
                var productSize = await _db.ProductsSizes
                    .FirstOrDefaultAsync(ps => ps.IdProduct == productId && ps.IdSize == sizeId);

                if (productSize == null)
                {
                    TempData["ErrorMessage"] = "selected size not found fot this product";
                    return RedirectToAction("ProductDetailsDrinks", new { id = productId });
                }

                double totalPrice = productSize.Price;
                List<int> selectedAddOns = new List<int>();

                // التحقق من وجود السكر إذا تم اختياره
                if (Suger.HasValue && Suger.Value != 0)
                {
                    var selectedSuger = await _db.ProductAddOns
                        .FirstOrDefaultAsync(f => f.Id == Suger.Value);

                    if (selectedSuger == null)
                    {
                        TempData["ErrorMessage"] = "suger selected not found ";
                        return RedirectToAction("ProductDetailsDrinks", new { id = productId });
                    }
                    totalPrice += selectedSuger.Price;
                    selectedAddOns.Add(selectedSuger.Id);
                }

                // التحقق من وجود الحليب إذا تم اختياره
                if (Milk.HasValue && Milk.Value != 0)
                {
                    var selectedMilk = await _db.ProductAddOns
                        .FirstOrDefaultAsync(f => f.Id == Milk.Value);

                    if (selectedMilk == null)
                    {
                        TempData["ErrorMessage"] = "milk selected not found";
                        return RedirectToAction("ProductDetailsDrinks", new { id = productId });
                    }
                    totalPrice += selectedMilk.Price;
                    selectedAddOns.Add(selectedMilk.Id);
                }
                // التحقق إذا كان المنتج موجوداً بالسلة مع نفس الخيارات
                var potentialItems = await _db.Baskets
                    .Include(b => b.BasketProductAddOns)
                    .Where(b => b.IdUser == userId.Value && b.IdProductSize == productSize.Id)
                    .ToListAsync();

                var existingBasketItem = potentialItems.FirstOrDefault(b =>
                    b.BasketProductAddOns.Count == selectedAddOns.Count &&
                    selectedAddOns.All(addOnId =>
                        b.BasketProductAddOns.Any(a => a.IdProductAddOns == addOnId))
                );

                if (existingBasketItem != null)
                {
                    // المنتج موجود بنفس الخيارات، زيادة الكمية
                    existingBasketItem.QuantityProduct += 1;

                    _db.Baskets.Update(existingBasketItem);
                    TempData["SuccessMessage"] = $"the quntity of product in cart is upgrid ({existingBasketItem.QuantityProduct} قطعة).";
                }
                else
                {
                    // إنشاء عنصر سلة جديد
                    var basketItem = new Basket
                    {
                        IdUser = userId.Value,
                        IdProductSize = productSize.Id,
                        QuantityProduct = 1,
                        AddedDate = DateTime.Now,
                        TotalPrice = totalPrice
                    };
                    _db.Baskets.Add(basketItem);
                    await _db.SaveChangesAsync();

                    // إضافة الإضافات المختارة فقط
                    foreach (var addOnId in selectedAddOns)
                    {
                        _db.BasketProductAddOns.Add(new BasketProductAddOn
                        {
                            IdBasket = basketItem.IdBasket,
                            IdProductAddOns = addOnId
                        });
                    }

                    TempData["SuccessMessage"] = "product add to cart success";
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("ProductDetailsDrinks", new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddDrinkesToBasket: {ex.Message}");
                TempData["ErrorMessage"] = "error happin when add the product to cart,try agin later.";
                return RedirectToAction("ProductDetailsDrinks", new { id = productId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSweetToBasket(int productId, int? sizeId, int? flavour)
        {
            // التحقق من وجود حجم مختار (إلزامي)
            if (!sizeId.HasValue || sizeId.Value == 0)
            {
                TempData["ErrorMessage"] = "select the size of product befor add to cart";
                return RedirectToAction("ProductDetailsSweets", new { id = productId });
            }

            // التحقق من تسجيل دخول المستخدم
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "sign in first to add products to cart.";
                return RedirectToAction("ProductDetailsSweets", new { id = productId });
            }

            try
            {
                // البحث عن ProductSize المطابق
                var productSize = await _db.ProductsSizes
                    .FirstOrDefaultAsync(ps => ps.IdProduct == productId && ps.IdSize == sizeId);

                if (productSize == null)
                {
                    TempData["ErrorMessage"] = "الحجم المحدد غير متوفر لهذا المنتج.";
                    return RedirectToAction("ProductDetailsSweets", new { id = productId });
                }

                // التحقق من وجود النكهة إذا تم اختيارها
                ProductAddOn? selectedFlavour = null;
                if (flavour.HasValue && flavour.Value != 0)
                {
                    selectedFlavour = await _db.ProductAddOns
                        .FirstOrDefaultAsync(f => f.Id == flavour.Value);

                    if (selectedFlavour == null)
                    {
                        TempData["ErrorMessage"] = "النكهة المحددة غير متوفرة";
                        return RedirectToAction("ProductDetailsSweets", new { id = productId });
                    }
                }

                // التحقق إذا كان المنتج موجوداً بالسلة مع نفس النكهة
                var existingBasketItem = await _db.Baskets
                    .Include(b => b.BasketProductAddOns)
                    .FirstOrDefaultAsync(b => b.IdUser == userId.Value &&
                                            b.IdProductSize == productSize.Id &&
                                            (selectedFlavour != null ?
                                                b.BasketProductAddOns.Any(a => a.IdProductAddOns == selectedFlavour.Id) :
                                                !b.BasketProductAddOns.Any()));

                if (existingBasketItem != null)
                {
                    existingBasketItem.QuantityProduct += 1;

                    _db.Baskets.Update(existingBasketItem);
                    TempData["SuccessMessage"] = $"تم تحديث كمية المنتج في السلة بنجاح ({existingBasketItem.QuantityProduct} قطعة).";
                }
                else
                {
                    // إنشاء عنصر سلة جديد
                    var basketItem = new Basket
                    {
                        IdUser = userId.Value,
                        IdProductSize = productSize.Id,
                        QuantityProduct = 1,
                        AddedDate = DateTime.Now,
                        TotalPrice = productSize.Price + (selectedFlavour?.Price ?? 0)
                    };
                    _db.Baskets.Add(basketItem);
                    await _db.SaveChangesAsync();

                    // إضافة النكهة إذا تم اختيارها
                    if (selectedFlavour != null)
                    {
                        var basketProductAddOn = new BasketProductAddOn
                        {
                            IdBasket = basketItem.IdBasket,
                            IdProductAddOns = selectedFlavour.Id
                        };
                        _db.BasketProductAddOns.Add(basketProductAddOn);
                    }

                    TempData["SuccessMessage"] = "تمت إضافة المنتج إلى السلة بنجاح.";
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("ProductDetailsSweets", new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddSweetToBasket: {ex.Message}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء محاولة إضافة المنتج للسلة. الرجاء المحاولة مرة أخرى.";
                return RedirectToAction("ProductDetailsSweets", new { id = productId });
            }
        }

        public IActionResult Menu()
        {
            // الحصول على المنتجات لكل فئة
            var category1Products = GetProductsByCategory(1);
            var category2Products = GetProductsByCategory(2);
            var category3Products = GetProductsByCategory(3);

            ViewBag.Category1Products = category1Products;
            ViewBag.Category2Products = category2Products;
            ViewBag.Category3Products = category3Products;

            return View();
        }

        public IActionResult Products()
        {
            var category4Products = GetProductsByCategory(4);

            ViewBag.Products = category4Products;


            return View();
        }
        private List<ProductViewModel> GetProductsByCategory(int categoryId)
        {
            return _db.Products
                .Where(p => p.IdCategory == categoryId)
                .Include(p => p.ProductsSizes)
                .ThenInclude(ps => ps.IdSizeNavigation)
                .Include(p => p.Reviews)
                .AsEnumerable()
                .Select(p => new ProductViewModel
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Picture = p.Picture,
                    IdCategory = p.IdCategory,
                    MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                })
                .ToList();
        }
        public async Task<IActionResult> ProductDetailsDrinksAsync(int id)

        {
                var product = _db.Products
                   .Include(p => p.ProductsSizes)
                   .Include(p => p.Reviews)
                   .AsEnumerable() 
                   .Select(p => new ProductViewModel
                   {
                       IdProduct = p.IdProduct,
                       NameProduct = p.NameProduct,
                       Picture = p.Picture,
                       CreatedDate = p.CreatedDate,
                       IdCategory = p.IdCategory,
                       About = p.About,
                       MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                       Prices = p.ProductsSizes.ToDictionary(ps => ps.IdSize, ps => ps.Price),
                       AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                   })
                   .FirstOrDefault(p => p.IdProduct == id);

                if (product == null)
                {
                    return NotFound();
                }

                var relatedProducts = _db.Products
                    .Where(p => p.IdCategory == product.IdCategory && p.IdProduct != id)
                    .Take(4)
                    .Select(p => new ProductViewModel
                    {
                        IdProduct = p.IdProduct,
                        NameProduct = p.NameProduct,
                        Picture = p.Picture,
                        IdCategory = p.IdCategory,
                        MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                    })
                    .ToList();

                var suger = _db.ProductAddOns
                    .Where(p => p.IdType == 1)
                    .ToList();

                var milk = _db.ProductAddOns
                    .Where(p => p.IdType == 2)
                    .ToList();

            ViewBag.Suger = suger;
            ViewBag.Milk = milk;
            ViewBag.Product = product;
                ViewBag.RelatedProducts = relatedProducts;

            var userId = HttpContext.Session.GetInt32("UserId");
            bool isLoggedIn = userId.HasValue;
            bool isFavorite = false;
            if (isLoggedIn)
            {
                isFavorite = await _db.Favorites.AnyAsync(f => f.IdUser == userId.Value && f.IdProduct == id);
            }

            ViewBag.IsUserLoggedIn = isLoggedIn;
            ViewBag.IsFavorite = isFavorite; // This is critical for the view
            return View();

            

        }
        public IActionResult ProductDetailsSweets(int id)
        {
            var product = _db.Products
               .Include(p => p.ProductsSizes)
               .Include(p => p.Reviews)
               .AsEnumerable() // التنفيذ على العميل بدلاً من الخادم
               .Select(p => new ProductViewModel
               {
                   IdProduct = p.IdProduct,
                   NameProduct = p.NameProduct,
                   Picture = p.Picture,
                   CreatedDate = p.CreatedDate,
                   IdCategory = p.IdCategory,
                   About = p.About,
                   MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                   Prices = p.ProductsSizes.ToDictionary(ps => ps.IdSize, ps => ps.Price),
                   AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
               })
               .FirstOrDefault(p => p.IdProduct == id);

            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = _db.Products
                .Where(p => p.IdCategory == product.IdCategory && p.IdProduct != id)
                .Take(4)
                .Select(p => new ProductViewModel
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Picture = p.Picture,
                    IdCategory = p.IdCategory,
                    MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                })
                .ToList();

            var flavour = _db.ProductAddOns
                .Where(p => p.IdType == 3)
                .ToList();

            ViewBag.Flavour = flavour;
            ViewBag.Product = product;
            ViewBag.RelatedProducts = relatedProducts;


            return View();

        }

        public IActionResult ProductDetails(int id)
        {
            var product = _db.Products
                .Include(p => p.ProductsSizes)
                .Include(p => p.Reviews)
                .AsEnumerable() 
                .Select(p => new ProductViewModel
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Picture = p.Picture,
                    CreatedDate = p.CreatedDate,
                    IdCategory = p.IdCategory,
                    About = p.About,
                    MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                    Prices = p.ProductsSizes.ToDictionary(ps => ps.IdSize, ps => ps.Price),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                })
                .FirstOrDefault(p => p.IdProduct == id);

            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = _db.Products
                .Where(p => p.IdCategory == product.IdCategory && p.IdProduct != id)
                .Take(4)
                .Select(p => new ProductViewModel
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Picture = p.Picture,
                    IdCategory = p.IdCategory,
                    MinPrice = p.ProductsSizes.Min(ps => ps.Price),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                })
                .ToList();


            ViewBag.Product = product;
            ViewBag.RelatedProducts = relatedProducts;

            return View();
        }

        public IActionResult Favorit()
        {
            return View();
        }

    }
}
