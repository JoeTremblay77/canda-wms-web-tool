using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CandaWebUtility.Models
{
    public class ConfirmEmailViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password1 is required")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        [Display(Name = "Password")]
        public string Password1 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password2 is required")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        [Display(Name = "Confirm Password")]
        public string Password2 { get; set; }

        public string UserName { get; set; }
    }

    //public class ExternalLoginConfirmationViewModel
    //{
    //    [Required]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}

    //public class ExternalLoginListViewModel
    //{
    //    public string ReturnUrl { get; set; }
    //}

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        public LoginViewModel()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.RememberMe = false;
            this.Warning = string.Empty;
        }

        public string Warning { get; set; }

        [Required]
        [Display(Name = "UserName")]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string FullName { get; set; }

        //[Required]
        //[Display(Name = "User Type")]
        //[RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        //public string UserType { get; set; }

        //public List<string> UserTypes { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string Code { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 9)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string Password1 { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password1", ErrorMessage = "The password and confirmation password do not match.")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string Password2 { get; set; }
    }

    public class SendCodeViewModel
    {
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }

        public string SelectedProvider { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        public string Provider { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public string ReturnUrl { get; set; }
    }
}