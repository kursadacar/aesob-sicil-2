using System;
using System.Web.Mvc;
using CTD.Data.UnitofWork;
using CTD.Service.Interfaces;
using CTD.Web.Framework.Controller;

namespace CTD.Sicil.Controllers
{
    [Authorize]
    public class HomeController : AdminController
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly IMakbuzDokumService _makbuzDokumService;
        private readonly ISicilService _sicilService;

        public HomeController(IUnitofWork uow, IKullaniciService kullaniciService, ISicilService sicilService,
            IMakbuzDokumService makbuzDokumService) : base(uow)
        {
            _kullaniciService = kullaniciService;
            _sicilService = sicilService;
            _makbuzDokumService = makbuzDokumService;
        }

        public ActionResult Index()
        {
            try
            {
                var liste = _makbuzDokumService.GunlukIslemAkisi(Accesses.Hak, Accesses.Id);
                ViewBag.MakbuzDokum = liste;
            }
            finally
            {
                //intentionally left blank
            }
            return View();
        }

        [HttpPost]
        public ActionResult HizliUyeArama(string arama)
        {
            var uyeler = _sicilService.GetirUyeler(arama);
            if (uyeler?.Count == 1)
            {
                return Json(uyeler, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return AjaxMessage("Bulunamadı", $"\"{arama}\" arama kriterlerine uygun üye bulunamadı. Lütfen girdiğiniz T.C. Kimlik No veya Sicil Numarasının doğru olduğundan emin olun.", Web.Framework.Enums.MessageTypes.info);
            }
        }

        public ActionResult Arama()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UnvanArama(string kriter)
        {
            var unvan = _sicilService.UnvanArama(kriter);
            var ds = "<table id='tblUnvan' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>SİCİL NO</th><th>ADI SOYADI</th><th>İŞYERİ ÜNVANI</th><th>MESLEK</th><th>ODASI</th><th>İLÇE</th></tr></thead><tbody>";
            foreach (var item in unvan)
                ds += "<tr href='http://myspace.com'><td><a href='/Sicil/SicilDetail/" + item.SicilNo + "'>" +
                      item.SicilNo + "</a></td><td>" + item.AdSoyad + "</td><td>" + item.IsyeriUnvani + "</td><td>" +
                      item.Meslek + "</td><td>" + item.Odasi + "</td><td>" + item.Ilce + "</td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }
    }
}