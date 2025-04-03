using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public  IActionResult AddEditCategory(CategoryVM category)
        {
        
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new ApiResponse(400, "Validation errors", errors));
            }

            try
            {
                if (category.Id == 0)
                {
                    var createdCategory =  _categoryService.CreateCategoryAsync(category);
                    return Json(new ApiResponse(200, "Category created successfully", createdCategory));
                }
                else
                {
                    _categoryService.UpdateCategoryAsync(category);
                    return Json(new ApiResponse(200, "Category updated successfully"));
                }
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
          
              
                return Json(new ApiResponse(500, "An error occurred while processing your request"));
            }
        }

        #endregion
    }
}
