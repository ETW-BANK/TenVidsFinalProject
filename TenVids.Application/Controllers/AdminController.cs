using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.Utilities;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles =$"{SD.AdminRole}")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;

        public AdminController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult Category()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Json(new ApiResponse(200,result:categories));
        }

        #endregion
    }
}
