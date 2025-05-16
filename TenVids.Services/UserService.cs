using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Data.Access.IRepo;
using TenVids.Services.IServices;
using TenVids.Utilities.FileHelpers;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly TenVidsApplicationContext _context;
        private readonly IPicService _picService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(UserManager<ApplicationUser> userManager,RoleManager<AppRole> roleManager,IMapper mapper,TenVidsApplicationContext context,IPicService picService,IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _picService = picService;
            _unitOfWork = unitOfWork;
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
                    user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null) return null;

                    user.UserName = model.Name.ToLower();
                    user.Email = model.Email;
                    user.Name = model.Name;

                    result = await _userManager.UpdateAsync(user);

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.Password);
                    }
                }

                if (!result.Succeeded) return null;

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

        public async Task<bool> LockUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return false; 
                }

                if (!user.LockoutEnabled)
                {
                    user.LockoutEnabled = true;
                    var enableLockoutResult = await _userManager.UpdateAsync(user);
                    if (!enableLockoutResult.Succeeded)
                    {
                        return false; 
                    }
                }

                var lockoutResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(1));
                return lockoutResult.Succeeded;
            }
            catch
            {
              
                return false;
            }
        }

        public async Task<bool> UnLockUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

                
                await _userManager.ResetAccessFailedCountAsync(user);

              
                user.LockoutEnabled = false;
                var updateResult = await _userManager.UpdateAsync(user);

                return updateResult.Succeeded;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            try
            {
                
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return false; 
                }

                
                var channel = await _context.Channels
                    .Include(c => c.Videos)
                    .FirstOrDefaultAsync(c => c.AppUserId == id);

                if (channel != null)
                {
                   
                    foreach (var video in channel.Videos)
                    {
                        _picService.DeletePhotoLocally(video.Thumbnail);
                    }

                  
                    _context.Videos.RemoveRange(channel.Videos);
                    await _context.SaveChangesAsync();

                 
                    _context.Channels.Remove(channel);
                    await _context.SaveChangesAsync();
                }

                
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            catch (DbUpdateConcurrencyException)
            {
                
                return false;
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }
    }
}
