using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TenVids.Repository.IRepository;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeService(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HomeVM> GoToHomeAsync(string page)
        {
            var home = new HomeVM();
            //var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();

            //if (string.IsNullOrEmpty(userId))
            //{
            //    throw new UnauthorizedAccessException("User not authenticated");
            //}

            home.Page= page ?? "Home";

            if (string.IsNullOrEmpty(page) || page.Equals("Home", StringComparison.OrdinalIgnoreCase))
            {
                var allCategories = await _unitOfWork.CategoryRepository.GetAllAsync();

                var categorylist = allCategories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList();

                categorylist.Insert(0, new SelectListItem
                {
                    Text = "All Categories",
                    Value = "0"
                });

                home.categoryList = categorylist;
            }

            return home;
        }

    }

  }

