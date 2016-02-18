using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BoilerplateWithBasicOWIN.DataAccess.Models;
using BoilerplateWithBasicOWIN.Models;
using BoilerplateWithBasicOWIN.Utility;
using Microsoft.AspNet.Identity;


namespace BoilerplateWithBasicOWIN.Controllers
{
    public class AccountController : BaseController
    {        
        public AccountController()
        {
            UserManager = new UserManager<Account>(_userdb);
        }

        public UserManager<Account> UserManager { get; private set; }

        [HttpGet]
        public ActionResult CreateAccount()
        {
            SignInAccountModel model = new SignInAccountModel();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount(SignInAccountModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: hash this with bcrypt
                BCryptHelper helper = new BCryptHelper();
                String passHash = helper.HashPassword(model.Password);
                Account account = new Account(model.UserName, model.Email, passHash, DateTime.Now, DateTime.Now );

                await _userdb.CreateAsync(account);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }      
    }
}