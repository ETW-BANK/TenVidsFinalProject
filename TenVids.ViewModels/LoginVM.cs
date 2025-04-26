using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace TenVids.ViewModels
{
    public class LoginVM
    {

        private string _userName;

        [Display(Name = "User Name or Email")]
        [Required(ErrorMessage = "User Name is required")]
        public string UserName
        {
            get => _userName;
            set => _userName =!string.IsNullOrEmpty(value)? value.ToLower():null; 
        }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [ValidateNever]
        public string RetunUrl { get; set; }
    }
}
