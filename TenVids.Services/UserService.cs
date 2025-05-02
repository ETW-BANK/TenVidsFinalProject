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

        public async Task<ApplicationUser> CreateUser(UserAddEditVM model)
        {
            try
            {
                IdentityResult result;
                ApplicationUser user;

                if (string.IsNullOrEmpty(model.Id))
                {
                    // Create new user
                    user = new ApplicationUser
                    {
                        UserName = model.Name.ToLower(),
                        Email = model.Email,
                        Name = model.Name
                    };

                    result = await _userManager.CreateAsync(user, model.Password);
                }
                else
                {
                    // Update existing user
                    user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null) return null;

                    user.UserName = model.Name.ToLower();
                    user.Email = model.Email;
                    user.Name = model.Name;

                    result = await _userManager.UpdateAsync(user);

                    // Update password if provided
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.Password);
                    }
                }

                if (!result.Succeeded) return null;

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, model.UserRoles);

                return user;
            }
            catch
            {
                return null;
            }
        }


        public async Task<(bool IsValid, UserAddEditVM Model)> ValidateUserForAddAsync(UserAddEditVM model)
        {
            var result = await AddUserAsync(model.Id); 
            bool isValid = true;

            if (model.Id == null) 
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    isValid = false;
                }

                if (model.UserRoles == null || model.UserRoles.Count == 0)
                {
                    isValid = false;
                }
                if(isValid&&NameExists(model.Name).GetAwaiter().GetResult()) 
                { 
                isValid=false;
                }
                if (isValid && EmailExists(model.Email).GetAwaiter().GetResult())
                {
                    isValid = false;
                }
            }

            return (isValid, result);
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

       public async Task<bool> EmailExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> NameExists(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            return user != null;
        }
    }
}
