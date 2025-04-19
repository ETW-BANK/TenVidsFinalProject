using Microsoft.AspNetCore.Mvc.Rendering;

namespace TenVids.ViewModels
{
    public class HomeVM
    {
        public string page { get; set; } 
        public IEnumerable<SelectListItem> categoryList { get; set; }
    }
}
