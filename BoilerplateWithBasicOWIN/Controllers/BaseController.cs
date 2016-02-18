using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BoilerplateWithBasicOWIN.DataAccess.Repository;
using BoilerplateWithBasicOWIN.CustomAttributes.FilterAttributes;

namespace BoilerplateWithBasicOWIN.Controllers
{
    [LayoutModelFilter]
    public class BaseController : Controller
    {

        protected UserRepository _userdb;
        protected String _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PostgresDotNet"].ConnectionString;

        public BaseController()
        {        
            _userdb = new UserRepository(_connectionString);
        }

    }
}