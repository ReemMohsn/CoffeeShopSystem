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
        public async Task<IActionResult> SendEmail(string email, string message)
        {
            var toEmail = "reemalthibani46@gmail.com";
            var subject = $"new message frome {email}";
            var body = $"Email: {email}\n\nMessage:\n{message}";

            using (var smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.Port = 587; // أو 465 حسب الخادم
                smtp.Credentials = new NetworkCredential("reemalthibani46", "xirj jdns wmiz qxut");
                smtp.EnableSsl = true; // استخدم SSL إذا لزم الأمر

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

            return RedirectToAction("Index"); // إعادة توجيه المستخدم بعد الإرسال
        }

        [HttpPost]
        public ActionResult CreateComment(Review comment)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("SignUp", "SignInUp"); // توجيه إلى صفحة تسجيل الدخول
            }
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var user = _db.Users.Find(userId); // تحقق مما إذا كان المستخدم موجودًا

            if (user == null)
            {
                return RedirectToAction("SignUp", "SignInUp"); // توجيه إلى صفحة تسجيل الدخول
            }
            comment.IdUser = userId; // احفظ UserId من الجلسة
            comment.ReviewDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.Reviews.Add(comment);
                _db.SaveChanges();
                return RedirectToAction("Index"); // أو أي إجراء آخر
            }
            ViewBag.productss = _db.Products.ToList();
            return View(comment);
        }
    }
}