using happinesCafe.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using happinesCafe.DATA;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace happinesCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CaffeeSystemContext _db;

        public HomeController(ILogger<HomeController> logger, CaffeeSystemContext context)
        {
            _db = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = _db.Products
                        .Where(p => p.IdCategory == 4)  // الفلترة حسب IdCategory == 4
    .Include(p => p.ProductsSizes)
        .ThenInclude(ps => ps.IdSizeNavigation)
        .Include(p => p.Reviews) // إضافة تحميل التقييمات
    .Take(3)
    .AsEnumerable() // التحويل إلى IEnumerable لتجنب مشاكل EF Core
    .Select(p => new ProductViewModel
    {
        IdProduct = p.IdProduct,
        NameProduct = p.NameProduct,
        Picture = p.Picture,
        CreatedDate = p.CreatedDate,
        IdCategory = p.IdCategory,
        About = p.About,
        Prices = p.ProductsSizes
                    .ToDictionary(ps => ps.IdSize, ps => ps.Price),
        MinPrice = p.ProductsSizes.Min(ps => ps.Price),
        AverageRating = p.Reviews.Any() ? // حساب متوسط التقييم
                         p.Reviews.Average(r => r.Rating) :0
        
    })
    .ToList();
            ViewBag.Products = products;

            // جلب الـ Reviews مع معلومات المستخدم والمنتج
            var reviews = _db.Reviews
                .Include(r => r.IdUserNavigation) // تحميل بيانات المستخدم
                .OrderByDescending(r => r.ReviewDate) // الأحدث أولاً
                .Select(r => new ReviewViewModel
                {
                    UserName = r.IdUserNavigation.NameUser,
                    UserImage = r.IdUserNavigation.PictuerUser, // افترض أن هذا اسم حقل الصورة
                    Rating = r.Rating,
                    ReviewText = r.Reviewtext,
                    ReviewDate = r.ReviewDate
                })
                .ToList();

            ViewBag.productss = _db.Products.ToList();

            ViewBag.Reviews = reviews;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(string email, string message)
        {
            try
            {
                var toEmail = "reemalthibani46@gmail.com";
                var subject = $"new message frome {email}";
                var body = $"Email: {email}\n\nMessage:\n{message}";

                using (var smtp = new SmtpClient("smtp.gmail.com"))
                {
                    smtp.Port = 587;
                    smtp.Credentials = new NetworkCredential("reemalthibani46", "xirj jdns wmiz qxut");
                    smtp.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(email),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(toEmail);

                    await smtp.SendMailAsync(mailMessage);
                }

                TempData["SuccessMessage"] = "Your message has been sent successfully! We'll get back to you soon.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to send your message. Please try again later.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateComment(Review comment)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "You must sign in first to submit a review.";
                return RedirectToAction("SignUp", "SignInUp");
            }
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var user = _db.Users.Find(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User account not found. Please sign in again.";
                return RedirectToAction("SignUp", "SignInUp");
            }
            comment.IdUser = userId;
            comment.ReviewDate = DateTime.Now;

            // إزالة أخطاء الـ navigation properties لأنها لا تُرسل من الفورم
            ModelState.Remove("IdUserNavigation");
            ModelState.Remove("IdProductNavigation");

            if (ModelState.IsValid)
            {
                _db.Reviews.Add(comment);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Thank you! Your review has been submitted successfully.";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Failed to submit review. Please check your input and try again.";
            ViewBag.productss = _db.Products.ToList();
            return RedirectToAction("Index");
        }
    }
}