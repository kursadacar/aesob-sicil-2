using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CTD.Core.Constants;
using CTD.Data.UnitofWork;
using CTD.Dto.ParamDto;
using CTD.Service.Interfaces;
using CTD.Utilities.Manager;
using CTD.Web.Framework.Controller;
using CTD.Web.Framework.Enums;

namespace CTD.Sicil.Controllers
{
    public class LoginController : PublicController
    {
        private readonly IKullaniciService _kullaniciService;

        public LoginController(IUnitofWork uow, IKullaniciService kullaniciService) : base(uow)
        {
            _kullaniciService = kullaniciService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginControl(LoginParamDto model)
        {
            if (Accesses.IsLogin() != ForbiddenAccessTypes.UnForbidden ||
                Accesses.IsLogin() == ForbiddenAccessTypes.IsLogout)
            {
                //var encryptedPassword = EnDeCode.Encrypt(model.Password, StaticParams.SifrelemeParametresi);
                //var user = _kullaniciService.KullaniciGetir(model.UserName, model.Password);
                //user.pass2 = encryptedPassword;
                //user.pass = "";
                //_kullaniciService.SifreGuncelle(user);
                //_uow.SaveChanges();
                //return AjaxMessage("Şifre Değiştirildi", $"Pass:{user.pass} -- Pass2:{user.pass2}", MessageTypes.info);
                //return AjaxMessage("Şifreniz", EnDeCode.Encrypt(model.Password, StaticParams.SifrelemeParametresi), MessageTypes.info);
                var person = _kullaniciService.KullaniciGetir(model.UserName, model.Password);
                if (person == null) return AjaxMessage("Uyarı", "Yanlış kullanıcı adı veya şifre", MessageTypes.danger);

                var ticket = new FormsAuthenticationTicket(1,
                    EnDeCode.Encrypt(person.Id.ToString(), StaticParams.SifrelemeParametresi), DateTime.Now,
                    DateTime.Now.AddDays(1), true,
                    EnDeCode.Encrypt(person.Id.ToString(), StaticParams.SifrelemeParametresi),
                    FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);

                var cookie = new HttpCookie(".u", encTicket);
                var kullaniciAdi = new HttpCookie("_adi", UtilityManager.Base64Encode(person.adi));
                var yetkisi = new HttpCookie("_hak", UtilityManager.Base64Encode(person.hak));
                var ipNo = new HttpCookie("_ip", UtilityManager.Base64Encode(person.ip));
                var resim = new HttpCookie("_resim", UtilityManager.Base64Encode(person.resim));
                var id = new HttpCookie("_id", UtilityManager.Base64Encode(person.Id.ToString()));

                cookie.HttpOnly = true;
                kullaniciAdi.HttpOnly = true;
                yetkisi.HttpOnly = true;
                ipNo.HttpOnly = true;
                resim.HttpOnly = true;
                id.HttpOnly = true;

                var expireDate = DateTime.Now.AddDays(1);

                cookie.Expires = expireDate;
                kullaniciAdi.Expires = expireDate;
                yetkisi.Expires = expireDate;
                ipNo.Expires = expireDate;
                resim.Expires = expireDate;
                id.Expires = expireDate;

                Response.Cookies.Add(cookie);
                Response.Cookies.Add(kullaniciAdi);
                Response.Cookies.Add(yetkisi);
                Response.Cookies.Add(ipNo);
                Response.Cookies.Add(resim);
                Response.Cookies.Add(id);

                return Json("/Home/Index", JsonRequestBehavior.AllowGet);
            }

            return AjaxMessage(MessageTitleTypes.Uyari, "Yanlış kullanıcı adı veya şifre", MessageTypes.danger);
        }

        public ActionResult Logout(string r)
        {
            var myCookies = Request.Cookies.AllKeys;
            foreach (var cookie in myCookies) Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            FormsAuthentication.SignOut();
            if (Request.IsAjaxRequest()) return AjaxMessage("Hata", "window.location.reload()", MessageTypes.func);
            Response.Redirect("/login/index");
            return Json("");
        }
    }
}