﻿using System.Web.Mvc;
using CTD.Core.Constants;
using CTD.Core.Entities;
using CTD.Data.UnitofWork;
using CTD.Service.Interfaces;
using CTD.Utilities.Manager;
using CTD.Web.Framework.Controller;

namespace CTD.Sicil.Controllers
{
    [Authorize]
    public class AyarlarController : PublicController
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly IMakbuzService _makbuzService;

        public AyarlarController(IUnitofWork uow, IKullaniciService kullaniciService, IMakbuzService makbuzService) :
            base(uow)
        {
            _kullaniciService = kullaniciService;
            _makbuzService = makbuzService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ControlPassword(string pass)
        {
            var sonuc = 0;
            var u = _kullaniciService.KullaniciGetir(Accesses.Id, pass);
            if (u != null)
            {
                var d = EnDeCode.Decrypt(u.pass2, StaticParams.SifrelemeParametresi);
                if (u != null && d == pass) sonuc = 1;
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangePassword(string pass, string newpass)
        {
            var sonuc = 0;
            var u = _kullaniciService.KullaniciGetir(Accesses.Id, pass);
            if (u != null)
            {
                var s = EnDeCode.Encrypt(newpass, StaticParams.SifrelemeParametresi);
                u.pass2 = s;
                _kullaniciService.SifreGuncelle(u);
                _uow.SaveChanges();
                sonuc = 1;
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakbuzTanimlama(int? sayi)
        {
            var m = _makbuzService.GetirKullaniciMakbuz(Accesses.Id);
            if (m != null)
            {
                var mm = new Makbuz {Id = m.Id, ONTAKI = m.ONTAKI, MAKBUZNO = m.MAKBUZNO, KULLANICI = m.KULLANICI};
                return Json(mm, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult MakbuzTanimlama2(Makbuz model)
        {
            var sonuc = 0;
            var m = _makbuzService.GetirKullaniciMakbuz(Accesses.Id, model.Id);
            if (m != null)
            {
                m.Id = model.Id;
                m.ONTAKI = model.ONTAKI;
                m.MAKBUZNO = model.MAKBUZNO;
                _makbuzService.MakbuzGuncelle(m);
                _uow.SaveChanges();
                sonuc = 1;
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }
    }
}