using System.ComponentModel.DataAnnotations;

namespace happinesCafe.Models
{
    public class EditProfileViewModel
    {
        public string NameUser { get; set; }

        //[DataType(DataType.Upload)]
        public IFormFile ProfileImage { get; set; } // 👈 الصورة كملف

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } // 👈 كلمة السر الحالية
        public string? PictuerUser { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; } // 👈 كلمة السر الجديدة

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } // 👈 تأكيد كلمة السر الجديدة
    }

}
