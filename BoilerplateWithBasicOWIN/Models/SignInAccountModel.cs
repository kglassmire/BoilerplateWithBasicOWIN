using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BoilerplateWithBasicOWIN.Models
{
    public class SignInAccountModel
    {
        [StringLength(1024, ErrorMessage = "Maximum length for this field is 1024 characters. Minimum length is 3.", MinimumLength = 3)]
        [RegularExpression("^[A-Za-z0-9]+$")]
        [Required]
        [Display(Name="Username")]
        public String UserName { get; set; }

        [EmailAddress]  
        [Display(Name="Email")]      
        public String Email { get; set; }

        [Required]        
        [Display(Name="Password")]
        public String Password { get; set; }
    }
}