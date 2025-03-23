using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            set => _userName = value?.ToLower(); 
        }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string RetunUrl { get; set; }
    }
}
