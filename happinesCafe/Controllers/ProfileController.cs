using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using happinesCafe.DATA;
using happinesCafe.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace happinesCafe.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly CaffeeSystemContext _context;

        public ProfileController(CaffeeSystemContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }



        // GET: Profile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.IdRoleNavigation)
                .Include(u=>u.StatusNavigation)
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // GET: Profile/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new EditProfileViewModel
            {
                NameUser = user.NameUser,
                  PictuerUser=user.PictuerUser,

            };
            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "IdRole", user.IdRole);
            return View(model);
        }

        // POST: Profile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
        if (!ModelState.IsValid)
    {
        return View(model);
    }
            var user = await _context.Users
               .Include(u => u.IdRoleNavigation)
               .FirstOrDefaultAsync(m => m.IdUser == HttpContext.Session.GetInt32("UserId"));

            // ✅ التحقق من كلمة المرور الحالية
            if (user.PasswordUser!=model.CurrentPassword)
    {
        ModelState.AddModelError("CurrentPassword", "كلمة السر الحالية غير صحيحة.");
        return View(model);
    }
    // ✅ تحديث الاسم
    user.NameUser = model.NameUser;

    // ✅ تحديث كلمة المرور (إذا أدخل المستخدم كلمة جديدة)
    if (!string.IsNullOrEmpty(model.NewPassword))
    {
        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "كلمتا السر غير متطابقتين.");
            return View(model);
        }

        user.PasswordUser=model.NewPassword;
            }

               string filename=string.Empty;
            //✅ تحديث الصورة الشخصية
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "imges");

                filename = model.ProfileImage.FileName;

                string filePath = Path.Combine(uploadsFolder, filename) ;
                model.ProfileImage.CopyTo(new FileStream(filePath,FileMode.Create));

                user.PictuerUser = filename;
            }
            //✅ تحديث الصورة الشخصية
            //if (model.ProfileImage != null)
            //{
            //    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "users");
            //    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
            //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //    {
            //        model.ProfileImage.CopyTo(fileStream);
            //    }

            //    user.PictuerUser = "/users/" + uniqueFileName;
            //}

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = user.IdUser });
        }




    
    }
}
