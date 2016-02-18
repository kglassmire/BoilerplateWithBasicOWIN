using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BoilerplateWithBasicOWIN.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            return View("Index");
        }
    }
}