using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TenVids.ViewModels.CustomValidation;

public class UserAddEditVM
{
    public string Id { get; set; }  // No [Required] attribute

    [Display(Name = "Name (Username)")]
    [StringCustomValidation(name: "Name", required: true, minLength: 3, maxLength: 15,
        regex: "^[a-zA-Z\\s]+$", regexErrorMessage: "Use letters only please")]
    public string Name { get; set; }

    [StringCustomValidation(name: "Email", required: true, minLength: 0, maxLength: 0,
        regex: @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", regexErrorMessage: "Invalid email format")]
    public string Email { get; set; }

    [StringCustomValidation(name: "Password", required: false, minLength: 8, maxLength: 15,
        regex: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$",
        regexErrorMessage: "Password must be 8–15 characters, include upper/lowercase letters and a number")]
    public string Password { get; set; }

    [Required(ErrorMessage = "At least one role is required")]
    [Display(Name = "Roles")]
    public List<string> UserRoles { get; set; }

    [ValidateNever]
    public List<string> ApplicationRoles { get; set; }
}