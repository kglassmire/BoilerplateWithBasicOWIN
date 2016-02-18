using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using BoilerplateWithBasicOWIN.DataAccess.Models;
using BoilerplateWithBasicOWIN.DataAccess.Repository;

[assembly:
    OwinStartup(typeof(BoilerplateWithBasicOWIN.Startup))]

namespace BoilerplateWithBasicOWIN
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuthentication(app);
        }

        public void ConfigureAuthentication(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login")
            });

            
        }




    }
}