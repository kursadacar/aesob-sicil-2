using System.Web.Mvc;
using System.Web.Routing;
using CTD.Data.UnitofWork;
using CTD.Web.Framework.Enums;

namespace CTD.Web.Framework.Controller
{
    public class AdminController : BaseController
    {
        public AdminController(IUnitofWork uow) : base(uow)
        {
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var forbiddenType = Accesses.IsLogin();
            if (forbiddenType != ForbiddenAccessTypes.UnForbidden && forbiddenType != ForbiddenAccessTypes.IsLogout)
            {
            }

            if (!(User.Identity.IsAuthenticated && forbiddenType == ForbiddenAccessTypes.UnForbidden &&
                  forbiddenType != ForbiddenAccessTypes.IsLogout))
            {
                var c = requestContext.RouteData.Values["controller"];
                var a = requestContext.RouteData.Values["action"];
                requestContext.RouteData.Values["controller"] = "Login";
                requestContext.RouteData.Values["action"] = "Logout";
                Response.Redirect("/login/logout?r=/" + c + "/" + a);
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //if (Request.UserHostAddress != "::1")
            //{
            //    var c = filterContext.RouteData.Values["controller"];
            //    var a = filterContext.RouteData.Values["action"];
            //    var IpAddress = Request.UserHostAddress;
            //}

            if (filterContext.Exception != null)
            {
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.Status = "500 Internal Server Error";
                filterContext.Result = AjaxMessage("Hata",
                    "Hata No :" + filterContext.Exception.HResult + "<br/> Hata Mesajı : " + filterContext.Exception.ToString(), MessageTypes.info);
                    //(filterContext.Exception.InnerException == null
                    //    ? filterContext.Exception.Message
                    //    : filterContext.Exception.InnerException.Message), MessageTypes.danger);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}