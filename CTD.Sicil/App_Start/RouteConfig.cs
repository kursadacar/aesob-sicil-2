using System.Web.Mvc;
using System.Web.Routing;

namespace CTD.Sicil
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Sicil", "{controller}/{action}/{sicilno}",
                new {controller = "Sicil", action = "Detail", sicilno = UrlParameter.Optional});
            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional});
        }
    }
}