using Microsoft.AspNetCore.Mvc.Rendering;

namespace happinesCafe.Models.Admin
{
    public class CreateProductViewModel
    {              
        
        public string Name { get; set; }        
        public int CategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public IFormFile ImageFile { get; set; }
        //public byte[]? imgpro { get; set; }
        public string ExistingPicture { get; set; } // To handle potential edits (though not in the initial create)
        public string Description { get; set; }
        public Dictionary<int, double> SizesAndPrices { get; set; } = new Dictionary<int, double>();
        public List<SelectListItem>? AvailableSizes { get; set; }
    }
}
