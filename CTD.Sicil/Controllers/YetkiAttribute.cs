using System.Web.Mvc;

namespace CTD.Sicil.Controllers
{
    public class YetkiAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
        }
    }
}