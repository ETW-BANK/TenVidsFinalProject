using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenVids.Models;
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
        public async Task<IActionResult> Category()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            CategoryVM categoryVM = new();

            if (id == null || id == 0)
            {

                return View(categoryVM);
            }
            else
            {

                var category = _categoryService.GetCategoryByIdAsync(categoryVM.Id);
                if (category == null)
                {
                    return NotFound();
                }

                categoryVM.Id = category.Id;
              
              

                return View(categoryVM);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                if (category.Id == 0)
                {
                    //await _categoryService.CreateCategoryAsync(category);
                    TempData["success"] = "Category created successfully.";
                }
                else
                {
                    _categoryService.UpdateCategoryAsync(category);
                    TempData["success"] = "Category updated successfully.";
                }

                return RedirectToAction(nameof(Category));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(category);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var category = _categoryService.GetCategoryByIdAsync(id.Value).Result;
            if (category == null) return NotFound();

            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var category = _categoryService.GetCategoryByIdAsync(id.Value);

            if (category == null)
            {
                TempData["error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            await _categoryService.DeleteCategoryAsync(category.Result);
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    

    #region API CALLS
    [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Json(new ApiResponse(200,result:categories));
        }


        //[HttpPost]
        //public  IActionResult AddEditCategory(Category category)
        //{
        
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToList();

        //        return Json(new ApiResponse(400, "Validation errors", errors));
        //    }

        //    try
        //    {
        //        if (category.Id == 0)
        //        {
        //            var createdCategory =  _categoryService.CreateCategoryAsync(category);
        //            return Json(new ApiResponse(200, "Category created successfully", createdCategory));
        //        }
        //        else
        //        {
        //            _categoryService.UpdateCategoryAsync(category);
        //            return Json(new ApiResponse(200, "Category updated successfully"));
        //        }
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return Json(new ApiResponse(404, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
          
              
        //        return Json(new ApiResponse(500, "An error occurred while processing your request"));
        //    }
        //}

        #endregion
    }
}
