using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BoilerplateWithBasicOWIN.DataAccess.Models
{
    public class Account : IUser
    {
        public Account()
        { }

        public Account(String userName, String email, String passHash, DateTime created, DateTime updated)
        {
            UserName = userName;
            Email = email;
            PassHash = passHash;
            Created = created;
            Updated = updated;
        }

        public Int64 Id { get; set; }

        [Required]
        [StringLength(1024, ErrorMessage = "Maximum length for this field is 1024 characters.", MinimumLength = 1)]
        public String UserName { get; set; }
        
        [Required]
        [StringLength(254, ErrorMessage = "Maximum length for this field is 254 characters.")]
        [EmailAddress(ErrorMessage = "Not a valid email address.")]
        public String Email { get; set; }

        public String PassHash { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        string IUser<string>.Id
        {
            get
            {
                return Id.ToString();
            }            
        }
    }
}