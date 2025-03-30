using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

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
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Json(new ApiResponse(200,result:categories));
        }

     
        [HttpPost]
        public async Task<IActionResult> AddEditCategory(CategoryVM category)
        {
            if(ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    var createdCategory = await _categoryService.CreateCategoryAsync(category);
                    return Json(new ApiResponse(200, "Category Created", createdCategory));
                }
                else
                {
                    var updatedCategory = await _categoryService.UpdateCategoryAsync(category);
                    return Json(new ApiResponse(200, "Category Updated", updatedCategory));
                }

            }
            return Json(new ApiResponse(400, "Something went wrong"));
        }

        #endregion
    }
}
