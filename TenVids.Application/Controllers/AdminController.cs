using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TenVids.Models;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;
using Xunit;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles = $"{SD.AdminRole}")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public AdminController(ICategoryService categoryService, IUserService userService)
        {
            _categoryService = categoryService;
            _userService = userService;
        }

        public async Task<IActionResult> AllUsers()
        {
            var result= await _userService.GetAllUsersAsync();
            return View(result);
        }
        

        public async Task<IActionResult> AddEditUser(string id)
        {
            var resukt= await _userService.AddUserAsync(id);

            return View(resukt);    
        }
     
    
        [HttpPost]
        public async Task<IActionResult> AddEditUser(UserAddEditVM model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                ModelState.Remove("Id");
            }

          
            model.ApplicationRoles = await _userService.GetApplicationRols();

         
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Password", "Password is required");
                    return View(model);
                }

                if (await _userService.NameExists(model.Name))
                {
                    ModelState.AddModelError("Name", "Username already exists");
                    return View(model);
                }

                if (await _userService.EmailExists(model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }
            }

    
            var result = await _userService.CreateUser(model);
            if (result == null)
            {
                ModelState.AddModelError("", "Failed to create user");
                return View(model);
            }

            TempData["success"] = $"User {(string.IsNullOrEmpty(model.Id) ? "created" : "updated")} successfully!";
            return RedirectToAction("AllUsers");
        }

        [HttpPost]

        public async Task<IActionResult> LockUser(string id)
        {
            var result=await _userService.LockUserAsync(id);
            if(!ModelState.IsValid) 
            
            {
                TempData["notification"] = "User Not Found";
                return RedirectToAction("AllUsers");
            }
           
           

            TempData["notification"] = "User Locked Sucessfully";

            return RedirectToAction("AllUsers");

        }
        [HttpPost]

        public async Task<IActionResult> UnLockUser(string id)
        {
            var result = await _userService.UnLockUserAsync(id);
            if (!ModelState.IsValid)

            {
                TempData["notification"] = "User not found";
                return RedirectToAction("AllUsers");
            }

            TempData["notification"] = "User UnLocked Sucessfully";

            return RedirectToAction("AllUsers");

        }
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            if (string.IsNullOrEmpty(request?.Id))
            {
                return Json(new { isSuccess = false, message = "User ID is required" });
            }

            try
            {
                var result = await _userService.DeleteUserAsync(request.Id);

                if (result)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        message = "User deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "User not found or could not be deleted"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        public class DeleteUserRequest
        {
            public string Id { get; set; }
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
    

