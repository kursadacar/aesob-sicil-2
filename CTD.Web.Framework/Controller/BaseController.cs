using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using CTD.Data.UnitofWork;
using CTD.Web.Framework.Enums;

namespace CTD.Web.Framework.Controller
{
    public class BaseController : System.Web.Mvc.Controller
    {
        public readonly IUnitofWork _uow;

        public BaseController(IUnitofWork uow)
        {
            _uow = uow;
        }

        public ActionResult AjaxMessage(string title, string content, MessageTypes state)
        {
            Response.StatusDescription = state.ToString();
            return Content(content + "##" + title, MediaTypeNames.Text.Plain);
        }

        public ActionResult AjaxSuccess()
        {
            return AjaxMessage(MessageTitleTypes.Tebrikler, "İşlem başarıyla gerçeleşti.", MessageTypes.success);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var validationErrors = (from item in ModelState.Values from error in item.Errors select error.ErrorMessage)
                .ToList();
            if (validationErrors.Count > 0 && Request.IsAjaxRequest() && validationErrors[0] != "")
                filterContext.Result = AjaxMessage(MessageTitleTypes.Uyari, validationErrors[0], MessageTypes.warning);
        }
    }
}