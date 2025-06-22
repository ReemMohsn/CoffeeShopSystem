using happinesCafe.DATA;
using happinesCafe.Models;
using happinesCafe.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace happinesCafe.Controllers
{
    public class SignInUpController : Controller
    {
        private readonly CaffeeSystemContext _db;
        private readonly IEmailService _emailService;

        public SignInUpController(CaffeeSystemContext db, IEmailService emailService)
        {

            _emailService = emailService;
            _db = db;

        }

    public IActionResult SignIn()
        {
            //var model = new User();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {

                var user = await _db.Users
                            .FirstOrDefaultAsync(u => u.EmailUser.ToLower() == email);

                if (user == null)
                {
                    ModelState.AddModelError("", "There is no account associated with this email");
                    //TempData["error"] = "There is no account associated with this email";
                    return View();
                }

                if (user.PasswordUser != password) 
                {
                    ModelState.AddModelError("", "The password is incorrect.");
                    //TempData["error"] = "The password is incorrect.";
                    return View(); 
                }
                if (user.Status == 2)
                {
                    ModelState.AddModelError("", "This account is blocked.");
                    //TempData["error"] = "This account is blocked.";
                    return View(); 
                }

                // حفظ بيانات المستخدم في الجلسة
                HttpContext.Session.SetInt32("UserId", user.IdUser); 
                HttpContext.Session.SetString("UserEmail", user.EmailUser); 
                HttpContext.Session.SetString("UserName", user.NameUser); 

                if (user.IdRole == 2)
                {
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    return RedirectToAction("Index", "ControlPanel"); 
                }
         
            
        }

        public IActionResult SignUp()
        {
            var model = new User();
            //3.تعيين القيم الافتراضية
            model.IdRole = 2;
            model.CreatdDate = DateTime.Now;
            model.Status = 1;

            return View(model);
        }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> SignUp(User model, string CofPW)
            {
            model.IdRoleNavigation = await _db.Roles.FindAsync(model.IdRole);
            model.StatusNavigation = await _db.UserStatuses.FindAsync(model.Status);

            //// إعادة تعيين حالة النموذج
            ModelState.Clear();
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                // 1. التحقق من تطابق كلمة المرور مع التأكيد
                if (model.PasswordUser != CofPW)
                {
                    ModelState.AddModelError("", "The password and confirmation do not match.");
                    return View(model);
                }

                // 2. التحقق من عدم وجود البريد الإلكتروني مسبقاً
                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.EmailUser.ToLower() == model.EmailUser.ToLower());
                if (existingUser != null)
                {
                    ModelState.AddModelError("EmailUser", "This email is already registered.");
                    return View(model);
                }

                // 4. إنشاء وإرسال رمز التحقق
                var otp = GenerateOTP();

                // تخزين المستخدم ورمز التحقق مؤقتاً في الجلسة
                var userJson = System.Text.Json.JsonSerializer.Serialize(model);
                HttpContext.Session.SetString("TempUser", userJson);
                HttpContext.Session.SetString("OTP", otp);

                // إرسال البريد الإلكتروني
                await _emailService.SendEmailAsync(model.EmailUser, "Verify Your Email", $"Your OTP is: {otp}");


                return RedirectToAction("Verify");
            }
            return View(model);

        }
           

        // GET: Verify
        public IActionResult Verify()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(string otp)
        {
            var savedOtp = HttpContext.Session.GetString("OTP");
            var userJson = HttpContext.Session.GetString("TempUser");

            if (string.IsNullOrEmpty(savedOtp)|| string.IsNullOrEmpty(userJson))
            {
                ModelState.AddModelError("", "The session has expired. Please sign up again.");
                return View();
            }
            if (otp != savedOtp)
            {
                ModelState.AddModelError("", "The verification code is incorrect.");
                return View();
            }

            var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJson);
            if (user==null)
            {
                ModelState.AddModelError("", "There id an error in bring the userdata!");
                return View();
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // تنظيف الجلسة
            HttpContext.Session.Remove("TempUser");
            HttpContext.Session.Remove("OTP");

            // تسجيل الدخول تلقائياً
            HttpContext.Session.SetInt32("UserId", user.IdUser);
            HttpContext.Session.SetString("UserEmail", user.EmailUser);
            HttpContext.Session.SetString("UserName", user.NameUser);

            return RedirectToAction("Welcome");
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("","please enter the Email adress");
                return View();
            }

            // البحث عن المستخدم بواسطة البريد الإلكتروني
            var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailUser.ToLower() == email.ToLower());

            // إذا لم يوجد مستخدم بهذا البريد
            if (user == null)
            {
                ModelState.AddModelError("", "There is no account associated with this email");
                return View();
            }

            // تحقق من حالة الحساب  (مثلاً، لا تسمح بإعادة التعيين لحساب محظور)
            if (user.Status == 2)
            {
                ModelState.AddModelError("", "Can not reset a password for this account,becouse it is bloked");
                return View();
            }

            /// إنشاء رمز استعادة(Token) وتاريخ انتهاء الصلاحية
            var resetToken = Guid.NewGuid().ToString("N");


            // تخزين الرمز في قاعدة البيانات 
            user.PasswordResetToken = resetToken;

            // إنشاء رابط الاستعادة  
            var resetLink = Url.Action("ResetPassword", "SignInUp", new { token = resetToken }, Request.Scheme);

            if (string.IsNullOrEmpty(resetLink))
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while generating the reset link";
                return View();
            }

            try
            {

                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                // إرسال البريد الإلكتروني
                await _emailService.SendEmailAsync(email, "Password Reset Request",
                    $"Please click this link to reset your password: {resetLink}");

                TempData["successMessage"] = "The password reset link has been sent to your email.";
            }
            catch (DbUpdateException dbEx)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the data. Please try again";
                return RedirectToAction("ForgetPassword");
            }
            catch (Exception ex)
            {
                user.PasswordResetToken = null;
                user.ResetTokenExpiry = null;
                try { await _db.SaveChangesAsync(); } catch {  }

                TempData["ErrorMessage"] = "An error occurred while sending the email. Please try again later.";
                return RedirectToAction("ForgetPassword"); 
            }

            return RedirectToAction("ForgetPassword");

        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "  The reset link is invalid or missing. Please request a new one.";
                return RedirectToAction("ForgetPassword");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
            if (user == null )
            {
                TempData["ErrorMessage"] = " The password reset link is invalid.";

                if (user != null)
                {
                    user.PasswordResetToken = null;
                    try { await _db.SaveChangesAsync(); } catch (Exception ex) { }
                }

                return RedirectToAction("ForgetPassword");
            }
            return View("ResetPassword", token);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["ErrorMessage"]= "جميع الحقول مطلوبة.";
                return View("ResetPassword", token);
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"]= "Passwords do not match.";
                return View("ResetPassword", token);
            }


            var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);

            if (user == null )
            {
                TempData["ErrorMessage"] = "The password reset link is invalid. Please request a new one.";
                if (user != null)
                {
                    user.PasswordResetToken = null;
                    try { await _db.SaveChangesAsync(); } catch (Exception ex) { }
                }
                return RedirectToAction("ForgetPassword"); 
            }

            user.PasswordUser = newPassword;

            //   مسح الرمز الانتهاء من قاعدة البيانات بعد الاستخدام 
            user.PasswordResetToken = null;

            try
            {
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                TempData["successMessage"] = "Your password has been reset successfully. Please sign in.";
                return RedirectToAction("SignIn");
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", "An error occurred while updating your password. Please try again.");
                return View("ResetPassword", token);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "A general error occurred. Please try again.");
                return View("ResetPassword", token);
            }
        }
        public IActionResult SignOut()
        {
            // حذف بيانات المستخدم من الجلسة
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserName");

            // توجيه المستخدم إلى صفحة تسجيل الدخول أو الصفحة الرئيسية
            return RedirectToAction("Index", "Home");
        }
        // دالة لإنشاء رمز تحقق عشوائي
        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // رمز مكون من 6 أرقام
        }
    }
}
