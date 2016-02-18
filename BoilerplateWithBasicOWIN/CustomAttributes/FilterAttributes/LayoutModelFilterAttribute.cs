using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BoilerplateWithBasicOWIN.Models;
using BoilerplateWithBasicOWIN.DataAccess.Repository;
using BoilerplateWithBasicOWIN.DataAccess.Models;

namespace BoilerplateWithBasicOWIN.CustomAttributes.FilterAttributes
{
    public class LayoutModelFilterAttribute : ActionFilterAttribute
    {
        private String _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PostgresDotNet"].ConnectionString;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // we need to create an instance of a viewmodel here and stuff it into the ViewBag for use on the Layout for every page.
            LayoutModel model = null;

            //TODO: Make this better.
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                UserRepository userRepo = new UserRepository(_connectionString);
                Account account = userRepo.FindByNameAsync(filterContext.HttpContext.User.Identity.Name).Result;
                model = new LayoutModel();
            }

            filterContext.Controller.ViewBag.LayoutModel = model;           
        }
    }
}