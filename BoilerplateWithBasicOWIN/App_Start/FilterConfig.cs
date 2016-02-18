using System.Web;
using System.Web.Mvc;
using BoilerplateWithBasicOWIN.CustomAttributes.FilterAttributes;

namespace BoilerplateWithBasicOWIN
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new DisableCacheFilterAttribute());

        }
    }
}
