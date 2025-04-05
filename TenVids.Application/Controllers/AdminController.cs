using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TenVids.Models;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles = $"{SD.AdminRole}")]
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
        public async Task<IActionResult> Upsert(int? id)
        {
            CategoryVM categoryVM = new();

            if (id == null || id == 0)
            {
               
                return View(categoryVM);
            }
            else
            {
                
                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null)
                {
                    return NotFound();
                }

                categoryVM.Id = category.Id;
                categoryVM.Name = category.Name;
                
                return View(categoryVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid category data.";
                TempData["CategoryModel"] = JsonSerializer.Serialize(model);
                return RedirectToAction(nameof(Upsert));
            }

            ErrorModel<Category> result;

            if (model.Id == 0)
            {
                result = await _categoryService.CreateCategoryAsync(model);
            }
            else
            {
                result = await _categoryService.UpdateCategoryAsync(model);
            }

            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["error"] = result.Message;
                TempData["CategoryModel"] = JsonSerializer.Serialize(model);
            }

            return RedirectToAction(nameof(Category));
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
            return RedirectToAction(nameof(Category));
        }

    }
}
    

