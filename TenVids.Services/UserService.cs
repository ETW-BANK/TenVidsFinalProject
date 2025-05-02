using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TenVids.Models;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager,RoleManager<AppRole> roleManager,IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserAddEditVM> AddUserAsync(string id)
        {
            var result = new UserAddEditVM
            {
                ApplicationRoles = await GetApplicationRols(),
                UserRoles = new List<string>() 
            };

            if (id != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                _mapper.Map(user, result);
                var userroles = await _userManager.GetRolesAsync(user);
                result.UserRoles = userroles.ToList();
            }

            return result;
        }

        public Task<ApplicationUser> CreateUser(UserAddEditVM model)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDisplayVM>> GetAllUsersAsync()
        {
           var result = new List<UserDisplayVM>();   

            var users=await _userManager.Users
                .Include(x=>x.Channel)
                .Where(x=>x.UserName!="admin")
                .ToListAsync();

            foreach (var user in users)
            {
                var userDisplayToAdd=new UserDisplayVM();  
                _mapper.Map(user,userDisplayToAdd);
                userDisplayToAdd.IsLocked=_userManager.IsLockedOutAsync(user).GetAwaiter().GetResult();
                userDisplayToAdd.AssignedRoles=_userManager.GetRolesAsync(user).GetAwaiter().GetResult(); 
                result.Add(userDisplayToAdd);   
            }

            return result;
        }

        public async Task<List<string>> GetApplicationRols()
        {
            return await _roleManager.Roles.Select(x=>x.Name).ToListAsync();
        }
    }
}
