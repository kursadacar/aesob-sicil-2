﻿using System.Web;
using System.Web.Mvc;

namespace CTD.Sicil.Controllers
{
    internal class _SessionControlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                if (!HttpContext.Current.Response.IsRequestBeingRedirected)
                    filterContext.HttpContext.Response.Redirect("/Home/Index");
        }
    }

    public class _SessionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}