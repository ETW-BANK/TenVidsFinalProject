using Microsoft.AspNetCore.Mvc.Rendering;

namespace TenVids.ViewModels
{
    public class HomeVM
    {
        public string Page { get; set; } 
        public IEnumerable<SelectListItem> categoryList { get; set; }
    }
}
