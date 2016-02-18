using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BoilerplateWithBasicOWIN.DataAccess.Models;
using BoilerplateWithBasicOWIN.Models;
using BoilerplateWithBasicOWIN.Utility;

namespace BoilerplateWithBasicOWIN.Controllers
{
    public class LoginController : BaseController
    {
        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            SignInAccountModel model = new SignInAccountModel();
            return View(model);
        }        

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection values)
        {
            SignInAccountModel model = new SignInAccountModel();
            if (values["UserName"] != null && values["Password"] != null)
            {
                model.UserName = values["UserName"];
                model.Password = values["Password"];
            }

            if (ModelState.IsValid)
            {
                Account account = await _userdb.FindByNameAsync(model.UserName);
                //verify the password to sign them in. 
                //Gonna have to wait for the password to be unhashed -- can't do this
                //quick with bcrypt but hey at least we are doing something secure here.
                BCryptHelper helper = new BCryptHelper();
                if (helper.Verify(model.Password, account.PassHash))
                {
                    //TODO: allow persistence
                    SignIn(account, false);

                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private void SignIn(Account account, bool isPersistent)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, account.UserName), new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()) }, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Role, "guest"));

            Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
        }

        public ActionResult SignOut()
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }

}