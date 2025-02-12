using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CandaWebUtility.Models
{
    public class EditUserViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string FullName { get; set; }

        public string Id { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        //[Required]
        [Display(Name = "User Type")]
        [RegularExpression("[^<>]+", ErrorMessage = "Invalid Symbols < >")]
        public string UserType { get; set; }

        //public List<string> UserTypes { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }
}