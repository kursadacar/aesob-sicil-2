using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CTD.Core.Entities;
using CTD.Core.Helpers;
using CTD.Data.UnitofWork;
using CTD.Dto.SingleDto;
using CTD.Service.Interfaces;
using CTD.Web.Framework.Controller;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;

namespace CTD.Sicil.Controllers
{
    [Authorize]
    public class SicilController : AdminController
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly ISicilService _sicilService;

        public SicilController(IUnitofWork uow, IKullaniciService kullaniciService, ISicilService sicilService) :
            base(uow)
        {
            _kullaniciService = kullaniciService;
            _sicilService = sicilService;
        }

        public ActionResult New(string dosya)
        {
            if (dosya == null)
            {
                var s = new SicilSingleDto();
                s.Sicil = new Core.Entities.Sicil();
                s.SicilMeslek = new SicilMeslek();
                return View(s);
            }

            var result = _sicilService.VerileriGetir(dosya);
            if (result == null) Response.Redirect("/Home");
            return View(result);
        }

        public ActionResult InsertSicilRecord(SicilSingleDto model)
        {
            var sicil = _sicilService.CheckSicilNo(model.Sicil.SICILNO);
            var tc = _sicilService.CheckTcKimlik(model.Sicil.TCKIMLIKNO);
            if (sicil == "sicilyok" && tc == "tcyok")
            {
                var kayittarihi = DateTime.Now;
                model.Sicil.KAYITTAR = kayittarihi;
                model.Sicil.KAYITEDEN = Accesses.KullaniciId;
                _sicilService.InsertSicil(model.Sicil);
                _uow.SaveChanges();
                model.SicilMeslek.SICILID = model.Sicil.Id;
                model.SicilMeslek.KAYITTAR = kayittarihi;
                model.SicilMeslek.VIZESURESIBITTAR = kayittarihi.AddMonths(6);
                model.SicilMeslek.KAYITEDEN = Accesses.KullaniciId;
                model.SicilMeslek.DEGEDEN = Accesses.Id;
                model.SicilMeslek.MERKEZ = true;
                model.SicilMeslek.ISADRES = model.Adres;
                var ilce = _sicilService.GetirIlce(model.SicilMeslek.ISADRESILCE).ILCE;
                var mahalle = _sicilService.GetirMahalle(model.SicilMeslek.ISADRESMAHALLE).MAHALLE;
                var cadsokbulv = _sicilService.GetirCadSokBulv(model.SicilMeslek.ISADRESCADSOKBULV).CADSOKBULV;
                model.SicilMeslek.ISADRES2 = mahalle + " " + cadsokbulv + " " + model.Adres + " " + ilce + "/ANTALYA";
                var nacesatir = _sicilService.GetirMeslekKodu(model.SicilMeslek.NACEKODU.ToString());
                model.SicilMeslek.MESLEK = nacesatir.MESLEKID;
                model.SicilMeslek.NACEKODU = nacesatir.Id;
                _sicilService.InsertSicilMeslek(model.SicilMeslek);
                _uow.SaveChanges();
                LogKaydiIsle(DateTime.Now, 1, "YENİ KAYIT", model.Sicil.Id, model.SicilMeslek.Id, Accesses.Id,
                    Accesses.Adi, int.Parse(model.SicilMeslek.MESLEK.ToString()));
                _uow.SaveChanges();
                return Json(model.Sicil.SICILNO, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public JsonResult VerileriGetir(string dosya)
        {
            if (dosya != null)
            {
                var result = _sicilService.VerileriGetir(dosya);
                if (result == null) Response.Redirect("/Home/Index");
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerileriGetirKisisel(string dosya)
        {
            return Json(_sicilService.VerileriGetirKisisel(dosya), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerileriGetirOda(string dosya)
        {
            return Json(_sicilService.VerileriGetirOda(dosya), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckSicilNo(int sicilno)
        {
            return Json(_sicilService.CheckSicilNo(sicilno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SicilNoKontrol(int? sicilno)
        {
            var uye = _sicilService.SicilNoKontrol(sicilno);
            return Json(uye, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckTcKimlik(string tcno)
        {
            return Json(_sicilService.CheckTcKimlik(tcno), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirSiniflar()
        {
            return Json(_sicilService.GetirSiniflar(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirOdalar()
        {
            return Json(_sicilService.GetirOdalar(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirMeslekler()
        {
            return Json(_sicilService.GetirMeslekler(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirIlceler()
        {
            return Json(_sicilService.GetirIlceler(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirIlceyeGoreMahalle(int? id)
        {
            return Json(_sicilService.GetirIlceyeGoreMahalle(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirMahalleyeGoreCadSokBulv(int? id)
        {
            return Json(_sicilService.GetirMahalleyeGoreCadSokBulv(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SicilDetail(int sicilno)
        {
            var result = _sicilService.SicilDetail(sicilno);
            return View(result);
        }

        [HttpPost]
        public ActionResult SicilGuncelle(SicilDto model)
        {
            var entity = _sicilService.SicilGetir(model.Sicil.Id);
            entity.DEGEDEN = Accesses.Id;
            entity.DEGTAR = DateTime.Now;
            entity.ADSOYAD = model.Sicil.ADSOYAD;
            entity.ANAADI = model.Sicil.ANAADI;
            entity.BABAADI = model.Sicil.BABAADI;
            entity.CEPTEL = model.Sicil.CEPTEL;
            entity.CINSIYET = model.Sicil.CINSIYET;
            entity.DOGTAR = model.Sicil.DOGTAR;
            entity.DOGYERILCE = model.Sicil.DOGYERILCE;
            entity.SICILNO = model.Sicil.SICILNO;
            entity.TCKIMLIKNO = model.Sicil.TCKIMLIKNO;
            entity.ACIKLAMA = model.Sicil.ACIKLAMA;
            _sicilService.SicilGuncelle(entity);
            _uow.SaveChanges();
            LogKaydiIsle(DateTime.Now, 16, "KİŞİSEL BİLGİLER", model.Sicil.Id, 0, Accesses.Id, Accesses.Adi, 0);
            _uow.SaveChanges();
            return AjaxSuccess();
        }

        public ActionResult _SicilDetailMeslekList(int? id)
        {
            var list = _sicilService.SicilMeslekListesi(id);
            return PartialView(list);
        }

        public JsonResult SicilDetailMeslek(int id)
        {
            var result = _sicilService.SicilDetailMeslek(id);
            if (result != null) return Json(result, JsonRequestBehavior.AllowGet);
            return null;
        }

        public JsonResult SicilMeslekDuzenle(int? id)
        {
            var result = _sicilService.SicilMeslekDuzenle(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateSicilMeslekRecord(SicilMeslekDto model)
        {
            var mevcut = _sicilService.SicilMeslekGetir(model.ID);
            SicilMeslekTablosunuSicilMeslekDegisiklik_LogaAktar(mevcut.SICILID);
            var degisenlistesi = new List<string>();
            if (mevcut.ISADRESILCE != model.ISADRESILCE)
            {
                mevcut.ISADRESILCE = model.ISADRESILCE;
                degisenlistesi.Add("İlçe");
            }

            if (mevcut.ISADRESMAHALLE != model.ISADRESMAHALLE)
            {
                mevcut.ISADRESMAHALLE = model.ISADRESMAHALLE;
                degisenlistesi.Add("Mahalle");
            }

            if (mevcut.ISADRESCADSOKBULV != model.ISADRESCADSOKBULV)
            {
                mevcut.ISADRESCADSOKBULV = model.ISADRESCADSOKBULV;
                degisenlistesi.Add("Cadde/Sokak/Bulvar");
            }

            if (mevcut.ISADRES != model.ISADRES)
            {
                mevcut.ISADRES = model.ISADRES;
                degisenlistesi.Add("Apartman/Bina/No");
            }

            var ilcem = _sicilService.GetirIlce(model.ISADRESILCE).ILCE;
            var mahallem = _sicilService.GetirMahalle(model.ISADRESMAHALLE).MAHALLE;
            var caddesokakm = _sicilService.GetirCadSokBulv(model.ISADRESCADSOKBULV).CADSOKBULV;
            mevcut.ISADRES2 = mahallem + " " + caddesokakm + " " + model.ISADRES + " " + ilcem + "/ANTALYA";
            if (mevcut.NACEKODU != model.NACEKODU)
            {
                mevcut.NACEKODU = model.NACEKODU;
                degisenlistesi.Add("NACE Kodu");
            }

            if (mevcut.ISYERIUNVANI != model.ISYERIUNVANI)
            {
                mevcut.ISYERIUNVANI = model.ISYERIUNVANI;
                degisenlistesi.Add("İşyeri Ünvanı");
            }

            if (mevcut.MESLEKODASI != model.MESLEKODASI)
            {
                mevcut.MESLEKODASI = model.MESLEKODASI;
                degisenlistesi.Add("Meslek Odası");
            }

            if (mevcut.MESLEK != model.MESLEK)
            {
                mevcut.MESLEK = model.MESLEK;
                degisenlistesi.Add("Meslek");
            }

            if (mevcut.SINIF != model.SINIF)
            {
                mevcut.SINIF = model.SINIF;
                degisenlistesi.Add("Sınıf");
            }

            mevcut.KAYITTAR = model.KAYITTAR;
            mevcut.SONDEGISTAR = model.SONDEGISTAR;
            if (mevcut.SONVIZEISTAR != model.SONVIZEISTAR)
            {
                mevcut.SONVIZEISTAR = model.SONVIZEISTAR;
                degisenlistesi.Add("Vize İşlem Tarihi");
            }

            if (mevcut.VIZESURESIBITTAR != model.VIZESURESIBITTAR)
            {
                mevcut.VIZESURESIBITTAR = model.VIZESURESIBITTAR;
                degisenlistesi.Add("Vize Bitiş Tarihi");
            }

            if (mevcut.MESLEKTERKTAR != model.MESLEKTERKTAR)
            {
                mevcut.MESLEKTERKTAR = model.MESLEKTERKTAR;
                degisenlistesi.Add("Meslek Terk Tarihi");
            }

            if (mevcut.MESLEKTERKNEDENI != model.MESLEKTERKNEDENI)
            {
                mevcut.MESLEKTERKNEDENI = model.MESLEKTERKNEDENI;
                degisenlistesi.Add("Meslek Terk Nedeni");
            }

            if (degisenlistesi.Count > 0)
            {
                mevcut.DEGEDEN = Accesses.Id;
                mevcut.DEGTAR = DateTime.Now;
            }

            mevcut.ACIKLAMA = model.ACIKLAMA;
            _sicilService.UpdateSicilMeslekRecord(mevcut);
            _uow.SaveChanges();
            var islemaciklamasi = "";
            for (var i = 0; i < degisenlistesi.Count; i++) islemaciklamasi += degisenlistesi[i] + ", ";
            islemaciklamasi += "alanları değiştirildi.";
            LogKaydiIsle(DateTime.Now, 0, islemaciklamasi, int.Parse(model.SICILID.ToString()),
                int.Parse(model.ID.ToString()), Accesses.Id, Accesses.Adi, int.Parse(model.MESLEK.ToString()));
            return Json(new {sicilid = mevcut.SICILID, meslekid = mevcut.Id, islemaciklamasi, meslek = mevcut.MESLEK},
                JsonRequestBehavior.AllowGet);
        }

        private int AdresDegisikligiVarmi(int? id, int? ilce, int? mahalle, int? cadde, string adres, string adres2)
        {
            var durum = 0;
            var mevcut = _sicilService.SicilMeslekGetir(id);
            if (mevcut.ISADRESILCE == ilce && mevcut.ISADRESMAHALLE == mahalle && mevcut.ISADRESCADSOKBULV == cadde &&
                mevcut.ISADRES == adres && mevcut.ISADRES2 == adres2)
                durum = 0;
            else
                durum = 1;
            return durum;
        }

        private int MeslekDegisikligiVarmi(int? id, int? nacekodu, string isyeriunvani, int? meslekodasi, int? meslek)
        {
            var durum = 0;
            var mevcut = _sicilService.SicilMeslekGetir(id);
            if (mevcut.NACEKODU == nacekodu && mevcut.ISYERIUNVANI == isyeriunvani &&
                mevcut.MESLEKODASI == meslekodasi && mevcut.MESLEK == meslek)
                durum = 0;
            else
                durum = 1;
            return durum;
        }

        private int VizeDegisikligiVarmi(int? id, DateTime? kayittarihi, DateTime? sondegistar, DateTime? sonvizeistar,
            DateTime? vizesuresibittar)
        {
            var durum = 0;
            var mevcut = _sicilService.SicilMeslekGetir(id);
            if (mevcut.KAYITTAR == kayittarihi && mevcut.SONDEGISTAR == sondegistar &&
                mevcut.SONVIZEISTAR == sonvizeistar && mevcut.VIZESURESIBITTAR == vizesuresibittar)
                durum = 0;
            else
                durum = 1;
            return durum;
        }

        private int TerkDegisikligiVarmi(int? id, DateTime? meslekterktar, int? meslekterknedeni)
        {
            var durum = 0;
            var mevcut = _sicilService.SicilMeslekGetir(id);
            if (mevcut.MESLEKTERKTAR == meslekterktar && mevcut.MESLEKTERKNEDENI == meslekterknedeni)
                durum = 0;
            else
                durum = 1;
            return durum;
        }

        private void SicilMeslekDegisiklikLogKaydet(SicilMeslek mevcut, int kullanici,
            out SicilMeslekDegisiklik_Log yeni)
        {
            yeni = new SicilMeslekDegisiklik_Log
            {
                SICILID = mevcut.SICILID, SICILMESLEKID = mevcut.Id, NACEKODU = mevcut.NACEKODU,
                ISYERIUNVANI = mevcut.ISYERIUNVANI, MESLEKODASI = mevcut.MESLEKODASI, MESLEK = mevcut.MESLEK,
                ISADRESILCE = mevcut.ISADRESILCE, ISADRESMAHALLE = mevcut.ISADRESMAHALLE,
                ISADRESCADSOKBULV = mevcut.ISADRESCADSOKBULV, ISADRES = mevcut.ISADRES, ISADRES2 = mevcut.ISADRES2,
                SINIF = mevcut.SINIF, KAYITTAR = mevcut.KAYITTAR, SONDEGISTAR = mevcut.SONDEGISTAR,
                SONVIZEISTAR = mevcut.SONVIZEISTAR, VIZESURESIBITTAR = mevcut.VIZESURESIBITTAR,
                MESLEKTERKTAR = mevcut.MESLEKTERKTAR, MESLEKTERKNEDENI = mevcut.MESLEKTERKNEDENI,
                ISLEMIYAPAN = kullanici,
                ISLEMTARIHI = DateHelper.GetDateTimeCultural(DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year),
                MERKEZ = mevcut.MERKEZ
            };
            _sicilService.KaydetSicilMeslekDegisiklik_log(yeni);
            _uow.SaveChanges();
        }

        public JsonResult NaceBilgileriGetir(string nacekodu)
        {
            var result = _sicilService.NaceBilgileriGetir(nacekodu);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MerkezBilgileriniGetir(int? id)
        {
            var result = _sicilService.MerkezBilgileriniGetir(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult YeniSubeEkle(SubeDto model)
        {
            var sm = new SicilMeslek
            {
                SICILID = model.SicilId, NACEKODU = model.NaceKodu, ISYERIUNVANI = model.IsyeriUnvani,
                MESLEKODASI = model.MeslekOdasi, MESLEK = model.Meslek, ISADRESILCE = model.IsAdresIlce,
                ISADRESMAHALLE = model.IsAdresMahalle, ISADRESCADSOKBULV = model.IsAdresCadSokBulv,
                ISADRES = model.IsAdres, SINIF = model.Sinif, KAYITTAR = model.KayitTar,
                VIZESURESIBITTAR = model.VizeSuresiBitTar, MESLEKTERKNEDENI = 0, KAYITEDEN = model.KayitEden,
                KAYITEDENTAR = model.KayitTar, ACIKLAMA = "Şube İlavesi", MERKEZ = false, MERKEZID = model.MerkezId
            };
            sm.KAYITEDEN = Accesses.Id;
            sm.KAYITEDENTAR = model.KayitTar;
            var ilce = _sicilService.GetirIlce(model.IsAdresIlce).ILCE;
            var mahalle = _sicilService.GetirMahalle(model.IsAdresMahalle).MAHALLE;
            var cadsokbulv = _sicilService.GetirCadSokBulv(model.IsAdresCadSokBulv).CADSOKBULV;
            sm.ISADRES2 = mahalle + " " + cadsokbulv + " " + model.IsAdres + " " + ilce + "/ANTALYA";
            _sicilService.InsertSicilMeslek(sm);
            var logsayisi = _sicilService.GetirSicilMeslekDegisiklik_LogSayisi(model.SicilId);
            if (logsayisi == 0) SicilMeslekTablosunuSicilMeslekDegisiklik_LogaAktar(model.SicilId);
            _uow.SaveChanges();
            LogKaydiIsle(DateTime.Now, 5, "ŞUBE İLAVESİ", sm.SICILID, sm.Id, Accesses.Id, Accesses.Adi,
                int.Parse(model.Meslek.ToString()));
            _uow.SaveChanges();
            return Json(new {sicilid = model.SicilId, meslekid = model.MerkezId}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SicilMeslekSil(int id)
        {
            var sm = _sicilService.SicilMeslekGetir(id);
            _sicilService.DeleteSicilMeslek(sm);
            LogKaydiIsle(DateTime.Now, 17, "İŞYERİ ve MESLEK SİLME", sm.SICILID, 0, Accesses.Id, Accesses.Adi,
                int.Parse(sm.MESLEK.ToString()));
            _uow.SaveChanges();
            return Json(sm.SICILID, JsonRequestBehavior.AllowGet);
        }

        public JsonResult YeniMeslekCombolariGetir()
        {
            var result = _sicilService.YeniMeslekCombolariGetir();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult YeniMeslekEkle(YeniMeslekEkleDto model)
        {
            var sm = new SicilMeslek
            {
                SICILID = model.newsicilid, NACEKODU = model.newnacekoduid, ISYERIUNVANI = model.newunvan,
                MESLEKODASI = model.newmeslekodasi, MESLEK = model.newmeslekno, ISADRESILCE = model.newilce,
                ISADRESMAHALLE = model.newmahalle, ISADRESCADSOKBULV = model.newcadde, ISADRES = model.newadres,
                SINIF = model.newsinif, KAYITTAR = model.newkayittarihi2, VIZESURESIBITTAR = model.newvizetarihi2,
                MESLEKTERKNEDENI = 0, KAYITEDENTAR = model.newkayittarihi2, ACIKLAMA = "Yeni Meslek İlavesi",
                MERKEZ = true, MERKEZID = 0, KAYITEDEN = Accesses.Id, DEGEDEN = Accesses.Id
            };
            var ilce = _sicilService.GetirIlce(model.newilce).ILCE;
            var mahalle = _sicilService.GetirMahalle(model.newmahalle).MAHALLE;
            var cadsokbulv = _sicilService.GetirCadSokBulv(model.newcadde).CADSOKBULV;
            sm.ISADRES2 = mahalle + " " + cadsokbulv + " " + model.newadres + " " + ilce + "/ANTALYA";
            _sicilService.InsertSicilMeslek(sm);
            var logsayisi = _sicilService.GetirSicilMeslekDegisiklik_LogSayisi(model.newsicilid);
            if (logsayisi == 0) SicilMeslekTablosunuSicilMeslekDegisiklik_LogaAktar(model.newsicilid);
            _uow.SaveChanges();
            LogKaydiIsle(DateTime.Now, 5, "MESLEK İLAVESİ", sm.SICILID, sm.Id, Accesses.Id, Accesses.Adi,
                int.Parse(sm.MESLEK.ToString()));
            _uow.SaveChanges();
            return Json(new {sicilid = model.newsicilid, meslekid = sm.Id}, JsonRequestBehavior.AllowGet);
        }

        private void SicilMeslekTablosunuSicilMeslekDegisiklik_LogaAktar(int newsicilid)
        {
            var liste = _sicilService.SicilMeslekMevcutDurum(newsicilid);
            foreach (var item in liste)
            {
                var yeni = new SicilMeslekDegisiklik_Log();
                SicilMeslekDegisiklikLogKaydet(item, Accesses.Id, out yeni);
            }
        }

        public JsonResult SicilSgkBilgi(int id)
        {
            var result = _sicilService.SicilSgkBilgi(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Report(string id, string sayi, string tarih, string konu, string ilgi, string anakurum,
            string altkurum, string metin, int? sicilid, int? durum)
        {
            if (durum == 2)
            {
                var lr = new LocalReport();
                var path = Path.Combine(Server.MapPath("~/Reports"), "rpYazismalar2.rdlc");
                if (System.IO.File.Exists(path))
                    lr.ReportPath = path;
                else
                    return View("SicilDetail");
                var p1 = new ReportParameter("sayi", sayi);
                var p2 = new ReportParameter("konu", konu);
                var p3 = new ReportParameter("tarih", tarih);
                var p4 = new ReportParameter("ilgi", ilgi);
                var p5 = new ReportParameter("anakurum", anakurum);
                var p6 = new ReportParameter("altkurum", altkurum);
                var p7 = new ReportParameter("metin", metin);
                ReportParameter p8 = null;
                ReportParameter p9 = null;
                if (Accesses.Adi != null)
                {
                    p8 = new ReportParameter("kullanici", Accesses.Adi);
                    if (Accesses.Hak == "admin")
                    {
                        p9 = new ReportParameter("unvan", "Admin");
                    }
                    else if (Accesses.Hak == "mudur")//TODO: Recheck
                        p9 = new ReportParameter("unvan", "Sicil Müdürü");
                    else
                        p9 = new ReportParameter("unvan", "Sicil Memuru");
                }

                lr.SetParameters(new[] {p1, p2, p3, p4, p5, p6, p7, p8, p9});
                lr.Refresh();
                var reportType = id;
                string mimeType;
                string encoding;
                string fileNameExtension;
                var deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + id + "</OutputFormat>" +
                                 "<PageWidth>8.5in</PageWidth>" + "<PageHeight>11in</PageHeight>" +
                                 "<MarginTop>0.5in</MarginTop>" + "<MarginLeft>0.5in</MarginLeft>" +
                                 "<MarginRight>0.5in</MarginRight>" + "<MarginBottom>0.5in</MarginBottom>" +
                                 "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                    out streams, out warnings);
                return File(renderedBytes, mimeType);
            }
            else
            {
                var lr = new LocalReport();
                var path = Path.Combine(Server.MapPath("~/Reports"), "rpYazismalar.rdlc");
                if (System.IO.File.Exists(path))
                    lr.ReportPath = path;
                else
                    return View("SicilDetail");
                var p1 = new ReportParameter("sayi", sayi);
                var p2 = new ReportParameter("konu", konu);
                var p3 = new ReportParameter("tarih", tarih);
                var p4 = new ReportParameter("ilgi", ilgi);
                var p5 = new ReportParameter("anakurum", anakurum);
                var p6 = new ReportParameter("altkurum", altkurum);
                var p7 = new ReportParameter("metin", metin);
                ReportParameter p8 = null;
                ReportParameter p9 = null;
                if (Accesses.Adi != null)
                {
                    p8 = new ReportParameter("kullanici", Accesses.Adi);
                    if (Accesses.Hak == "admin")
                    {
                        p9 = new ReportParameter("unvan", "Admin");
                    }
                    else if (Accesses.Hak == "mudur")
                    {
                        p9 = new ReportParameter("unvan", "Sicil Müdürü");
                    }
                    else
                    {
                        p9 = new ReportParameter("unvan", "Sicil Memuru");
                    }
                }

                lr.SetParameters(new[] {p1, p2, p3, p4, p5, p6, p7, p8, p9});
                if (sicilid != null)
                {
                    var meslekler = _sicilService.YazismaReportLists(int.Parse(sicilid.ToString()));
                    if (meslekler != null)
                    {
                        var rdsMeslekler = new ReportDataSource("Meslekler", meslekler);
                        lr.DataSources.Add(rdsMeslekler);
                    }
                }

                lr.Refresh();
                var reportType = id;
                string mimeType;
                string encoding;
                string fileNameExtension;
                var deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + id + "</OutputFormat>" +
                                 "<PageWidth>8.5in</PageWidth>" + "<PageHeight>11in</PageHeight>" +
                                 "<MarginTop>0.5in</MarginTop>" + "<MarginLeft>0.5in</MarginLeft>" +
                                 "<MarginRight>0.5in</MarginRight>" + "<MarginBottom>0.5in</MarginBottom>" +
                                 "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                    out streams, out warnings);
                return File(renderedBytes, mimeType);
            }
        }

        public ActionResult ReportSicilTasdiknamesi(int id)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpSicilTasdiknamesi.rdlc");
            if (System.IO.File.Exists(path))
                lr.ReportPath = path;
            else
                return null;
            var sicilmeslek = _sicilService.SicilMeslekGetir(id);
            var sicil = _sicilService.SicilGetir(sicilmeslek.SICILID);
            var odaadi = _sicilService.GetirOda(int.Parse(sicilmeslek.MESLEKODASI.ToString()));
            var nace = _sicilService.NaceBilgileriGetir2(int.Parse(sicilmeslek.NACEKODU.ToString()));
            lr.DataSources.Clear();
            var prAdSoyad = new ReportParameter("prAdSoyad", sicil.ADSOYAD);
            var prSicilNo = new ReportParameter("prSicilNo", sicil.SICILNO.ToString());
            var prTCKimlikNo = new ReportParameter("prTCKimlikNo", sicil.TCKIMLIKNO);
            var prBabaAdi = new ReportParameter("prBabaAdi", sicil.BABAADI);
            var prAnneAdi = new ReportParameter("prAnneAdi", sicil.ANAADI);
            var prDogumYeri = new ReportParameter("prDogumYeri", sicil.DOGYERILCE);
            var prDogumTarihi = new ReportParameter("prDogumTarihi", sicil.DOGTAR?.ToString("dd.MM.yyyy"));
            var prUnvan = new ReportParameter("prUnvan", sicilmeslek.ISYERIUNVANI);
            var prAdres = new ReportParameter("prAdres", sicilmeslek.ISADRES2);
            var prOda = new ReportParameter("prOda", odaadi);
            var prKayitTarihi = new ReportParameter("prKayitTarihi", sicilmeslek.KAYITTAR?.ToString("dd.MM.yyyy"));
            var prTerkTarihi = new ReportParameter("prTerkTarihi", sicilmeslek.MESLEKTERKTAR?.ToString("dd.MM.yyyy"));
            var prNaceKodu = new ReportParameter("prNaceKodu", nace.NACE);
            var prNaceTanimi = new ReportParameter("prNaceTanimi", nace.TANIMI);
            lr.SetParameters(new[]
            {
                prAdSoyad, prSicilNo, prTCKimlikNo, prBabaAdi, prAnneAdi, prDogumYeri, prDogumTarihi, prUnvan, prAdres,
                prOda, prKayitTarihi, prTerkTarihi, prNaceKodu, prNaceTanimi
            });
            lr.Refresh();
            var reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            var deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + reportType + "</OutputFormat>" +
                             "<PageWidth>8.5in</PageWidth>" + "<PageHeight>11in</PageHeight>" +
                             "<MarginTop>0.5in</MarginTop>" + "<MarginLeft>0.5in</MarginLeft>" +
                             "<MarginRight>0.5in</MarginRight>" + "<MarginBottom>0.5in</MarginBottom>" +
                             "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult ReportSuretDilekcesi(int id)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpSuretDilekcesi.rdlc");
            if (System.IO.File.Exists(path))
                lr.ReportPath = path;
            else
                return null;
            var sicilmeslek = _sicilService.SicilMeslekGetir(id);
            var sicil = _sicilService.SicilGetir(sicilmeslek.SICILID);
            var meslek = _sicilService.GetirMeslek(int.Parse(sicilmeslek.MESLEK.ToString()));
            lr.DataSources.Clear();
            var prAdSoyad = new ReportParameter("prAdSoyad", sicil.ADSOYAD);
            var prSicilNo = new ReportParameter("prSicilNo", sicil.SICILNO.ToString());
            var prTCKimlikNo = new ReportParameter("prTCKimlikNo", sicil.TCKIMLIKNO);
            var prMeslegi = new ReportParameter("prMeslegi", meslek);
            lr.SetParameters(new[] {prAdSoyad, prSicilNo, prTCKimlikNo, prMeslegi});
            lr.Refresh();
            var reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            var deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + reportType + "</OutputFormat>" +
                             "<PageWidth>8.5in</PageWidth>" + "<PageHeight>11in</PageHeight>" +
                             "<MarginTop>0.5in</MarginTop>" + "<MarginLeft>0.5in</MarginLeft>" +
                             "<MarginRight>0.5in</MarginRight>" + "<MarginBottom>0.5in</MarginBottom>" +
                             "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult SicilBilgiFormu(string id, int sayi)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpIslemBilgiForm.rdlc");
            if (System.IO.File.Exists(path)) lr.ReportPath = path;
            ReportParameter p1 = null;
            ReportParameter p2 = null;
            ReportParameter p3 = null;
            ReportParameter p4 = null;
            ReportParameter p5 = null;
            ReportParameter p6 = null;
            ReportParameter p7 = null;
            ReportParameter p8 = null;
            var s = _sicilService.SicilGetir(sayi);
            if (s != null)
            {
                p1 = new ReportParameter("adisoyadi", s.ADSOYAD);
                p2 = new ReportParameter("tckimlik", s.TCKIMLIKNO);
                p3 = new ReportParameter("sicilno", s.SICILNO.ToString());
                p4 = new ReportParameter("babaadi", s.BABAADI);
                p5 = new ReportParameter("dogumyeri", s.DOGYERILCE);
                p6 = new ReportParameter("dogumtarihi", s.DOGTAR?.ToString("dd.MM.yyyy"));
                p7 = new ReportParameter("kullanici", Accesses.Adi);
                if (Accesses.Hak == "admin")
                {
                    p8 = new ReportParameter("unvan", "Admin");
                }
                else if (Accesses.Hak == "mudur")
                {
                    p8 = new ReportParameter("unvan", "Sicil Müdürü");
                }
                else
                {
                    p8 = new ReportParameter("unvan", "Sicil Memuru");
                }
            }

            lr.SetParameters(new[] {p1, p2, p3, p4, p5, p6, p7, p8});
            var t = _sicilService.SonSicilMeslekListe(sayi);
            var rdsSicilMeslek = new ReportDataSource("SicilMeslekAktif", t);
            lr.DataSources.Add(rdsSicilMeslek);
            var t2 = _sicilService.SonSicilMeslekDegisiklikListe(sayi);
            var rdsSicilMeslekDegisiklik = new ReportDataSource("SicilMeslekDegisiklik", t2);
            lr.DataSources.Add(rdsSicilMeslekDegisiklik);
            lr.Refresh();
            var reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;
            var deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + id + "</OutputFormat>" +
                             "<PageWidth>8.5in</PageWidth>" + "<PageHeight>11in</PageHeight>" +
                             "<MarginTop>0.3in</MarginTop>" + "<MarginLeft>0.5in</MarginLeft>" +
                             "<MarginRight>0.5in</MarginRight>" + "<MarginBottom>0.3in</MarginBottom>" +
                             "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult ReportKayitKapak(int id)
        {
            var sicilmeslek = _sicilService.SicilDetailMeslek(id);
            var sicil = _sicilService.SicilDtoGetir(sicilmeslek.SicilId);
            var STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
            var fontNormal = new Font(STF_Helvetica_Turkish, 10, Font.NORMAL);
            var normalFont = new Font(Font.FontFamily.COURIER, 10f, Font.NORMAL);
            var tahomaNormal = FontFactory.GetFont("Tahoma", "CP1254", BaseFont.NOT_EMBEDDED, 10f);
            tahomaNormal.SetStyle(0);
            var proximanovaFont = new Font(BaseFont.CreateFont(
                Server.MapPath("~/fonts/proximanova-regular-webfont.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED));
            var proximanovaFont10 =
                new Font(
                    BaseFont.CreateFont(Server.MapPath("~/fonts/proximanova-regular-webfont.ttf"), BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED), 10);
            var proximanovaFontBold = new Font(BaseFont.CreateFont(
                Server.MapPath("~/fonts/proximanova-bold-webfont.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED));
            var pdfDoc = new Document(PageSize.A4, 40, 25, 25, 15);
            var pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            var chunk = new Chunk();
            pdfDoc.Add(chunk);
            var tableHeader = new PdfPTable(4);
            tableHeader.WidthPercentage = 100;
            tableHeader.SpacingBefore = 20f;
            tableHeader.SpacingAfter = 30f;
            float[] widths = {60f, 180f, 250f, 80f};
            tableHeader.SetWidths(widths);
            var cellHeader = new PdfPCell();
            var img = Image.GetInstance(Server.MapPath("~/Images/logo.jpg"));
            img.ScaleToFit(50, 50);
            cellHeader.Border = 1;
            cellHeader.Border = Rectangle.BOTTOM_BORDER;
            cellHeader.PaddingBottom = 10f;
            cellHeader.AddElement(img);
            tableHeader.AddCell(cellHeader);
            cellHeader = new PdfPCell(new Phrase("ANTALYA\nESNAF VE SANATKARLAR\nSİCİL MÜDÜRLÜĞÜ", proximanovaFont));
            cellHeader.PaddingTop = 10f;
            cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeader.Border = 1;
            cellHeader.Border = Rectangle.BOTTOM_BORDER;
            tableHeader.AddCell(cellHeader);
            cellHeader = new PdfPCell(new Phrase("KAYIT KAPAK SAYFASI", proximanovaFontBold));
            cellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellHeader.Border = 1;
            cellHeader.Border = Rectangle.BOTTOM_BORDER;
            tableHeader.AddCell(cellHeader);
            var tarih = Convert.ToString(DateTime.Now);
            cellHeader = new PdfPCell(new Phrase(tarih, proximanovaFont));
            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellHeader.Border = 1;
            cellHeader.Border = Rectangle.BOTTOM_BORDER;
            tableHeader.AddCell(cellHeader);
            pdfDoc.Add(tableHeader);
            var table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            widths = new[] {120f, 400f};
            table.SetWidths(widths);
            var cell = new PdfPCell();
            cell = new PdfPCell(new Phrase("Adı ve Soyadı", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 1;
            cell.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + sicil.AdSoyad, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
            cell.Padding = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("T.C. Kimlik No", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.LEFT_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + sicil.TCKimlikNo, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.RIGHT_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Esnaf Sicil No", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.LEFT_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + sicil.SicilNo, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.RIGHT_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Baba Adı / Ana Adı", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.LEFT_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + sicil.BabaAdi + " / " + sicil.AnaAdi, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.RIGHT_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Doğum Yeri / Tarihi", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(": " + sicil.DogYerIlce + " / " + sicil.DogTrh, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 1;
            cell.Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Kayıt Tarihi", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            var debugDate = sicilmeslek.KayitTar?.ToString("dd.MM.yyyy");
            cell = new PdfPCell(new Phrase(sicilmeslek.KayitTar?.ToString("dd.MM.yyyy"), proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("İşyeri Ünvanı", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(sicilmeslek.IsyeriUnvani, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("NACE Kodu", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(
                new Phrase(sicilmeslek.NaceAltiliKod + " - " + sicilmeslek.NaceTanim, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Mesleği", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(sicilmeslek.MeslekKodu + " - " + sicilmeslek.MeslekAdi, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Sınıfı / Meslek Odası", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(sicilmeslek.SinifString + " - " + sicilmeslek.OdaKisaAd, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("İşyeri Adresi", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(sicilmeslek.IsAdres2, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("İşlemi Yapan", proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(sicilmeslek.KayitEden, proximanovaFont10));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;
            table.AddCell(cell);
            pdfDoc.Add(table);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=KayitKapakSayfasi.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return View();
        }

        public List<SicilMeslekDegisiklik_Log> SilSicilMeslekDegisiklik_Log(int sicilid)
        {
            var model = new List<SicilMeslekDegisiklik_Log>();
            model = _sicilService.GetirSicilMeslekDegisiklik_Log(sicilid);
            return model;
        }

        [HttpPost]
        public JsonResult DetayliArama(string adsoyad, string anneadi, string babaadi, string meslekodasi, string meslek)
        {
            var arama = _sicilService.DetayliArama(adsoyad, anneadi, babaadi, meslekodasi, meslek);
            var ds = "<table id='tblDetayliArama' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>SİCİL NO</th><th>ADI SOYADI</th><th>TC KİMLİK NO</th><th>BABA ADI</th><th>ANNE ADI</th><th>DOĞUM TARİHİ</th></tr></thead><tbody>";
            foreach (var item in arama)
                ds += "<tr href='http://myspace.com'><td><a href='/Sicil/SicilDetail/" + item.SicilNo + "'>" +
                      item.SicilNo + "</a></td><td>" + item.AdSoyad + "</td><td>" + item.TcKimlikNo + "</td><td>" +
                      item.BabaAdi + "</td><td>" + item.AnneAdi + "</td><td>" + item.DogumTarihi + "</td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        private void LogKaydiIsle(DateTime islemtarihi, int islemturuid, string islemturu, int sicilid,
            int sicilmeslekid, int userid, string user, int meslekid)
        {
            var log = new TahsilatIslemleriDto
            {
                IslemTarihi = islemtarihi, IslemTuruId = islemturuid, IslemTuru = islemturu, SicilId = sicilid,
                SicilMeslekId = sicilmeslekid, UserId = userid, User = user,
                Meslek = _sicilService.GetirMeslek(meslekid)
            };
            _sicilService.InsertTahsilatIslemleri(log);
            _uow.SaveChanges();
        }

        public List<string> VizesiGecenler()
        {
            var vizesigelenler = _sicilService.VizesiGelenler();
            return null;
        }

        public JsonResult GetirIlceyeGoreUyeler(int id)
        {
            var uyeler = _sicilService.GetirIlceyeGoreUyeler(id);
            var ds = "<table id='tblUyeler' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>SİCİL NO</th><th>ADI SOYADI</th><th>TC KİMLİK NO</th><th>CEP TELEFONU</th></tr></thead><tbody>";
            var veri = 0;
            foreach (var item in uyeler)
            {
                foreach (var classA in item)
                    if (veri == 0)
                    {
                        ds += "<tr href='http://myspace.com'><td>" + classA.SicilNo + "</td><td>" + classA.AdSoyad +
                              "</td><td>" + classA.TCKimlikNo + "</td><td>" + classA.CepTel + "</td></tr>";
                        veri = item.Key;
                    }

                veri = 0;
            }

            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Listele(int? ilcem, int? mahallem, int? caddem)
        {
            var uyeler = _sicilService.Listele(ilcem, mahallem, caddem);
            var ds = "<table id='tblUyeler' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>SİCİL NO</th><th>ADI SOYADI</th><th>TC KİMLİK NO</th><th>CEP TELEFONU</th><th>MESLEK</th><th>İŞ ADRESİ</th></tr></thead><tbody>";
            var veri = 0;
            foreach (var item in uyeler)
            {
                foreach (var classA in item)
                    if (veri == 0)
                    {
                        ds += "<tr href='http://myspace.com'><td>" + classA.SicilNo + "</td><td>" + classA.AdSoyad +
                              "</td><td>" + classA.TCKimlikNo + "</td><td>" + classA.CepTel + "</td><td>" +
                              classA.Meslek + "</td><td>" + classA.IsAdres + "</td></tr>";
                        veri = item.Key;
                    }

                veri = 0;
            }

            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeleV2(int? ilcem, int? mahallem, int? caddem)
        {
            return Json(_sicilService.ListeleV2(ilcem, mahallem, caddem), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListeleOdaUye(int? odam)
        {
            var uyeler = _sicilService.ListeleOdaUye(odam);
            var ds = "<table id='tblUyeler' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>SİCİL NO</th><th>ADI SOYADI</th><th>TC KİMLİK NO</th><th>CEP TELEFONU</th><th>MESLEK</th><th>İŞ ADRESİ</th></tr></thead><tbody>";
            var veri = 0;
            foreach (var item in uyeler)
            {
                foreach (var classA in item)
                    if (veri == 0)
                    {
                        ds += "<tr href='http://myspace.com'><td>" + classA.SicilNo + "</td><td>" + classA.AdSoyad +
                              "</td><td>" + classA.TCKimlikNo + "</td><td>" + classA.CepTel + "</td><td>" +
                              classA.Meslek + "</td><td>" + classA.IsAdres + "</td></tr>";
                        veri = item.Key;
                    }

                veri = 0;
            }

            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListeleMeslekUye(int? meslek)
        {
            var uyeler = _sicilService.ListeleMeslekUye(meslek);
            var ds = "<table id='tblUyeler' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>SİCİL NO</th><th>ADI SOYADI</th><th>TC KİMLİK NO</th><th>CEP TELEFONU</th><th>MESLEK</th><th>İŞ ADRESİ</th></tr></thead><tbody>";
            var veri = 0;
            foreach (var item in uyeler)
            {
                foreach (var classA in item)
                    if (veri == 0)
                    {
                        ds += "<tr href='http://myspace.com'><td>" + classA.SicilNo + "</td><td>" + classA.AdSoyad +
                              "</td><td>" + classA.TCKimlikNo + "</td><td>" + classA.CepTel + "</td><td>" +
                              classA.Meslek + "</td><td>" + classA.IsAdres + "</td></tr>";
                        veri = item.Key;
                    }

                veri = 0;
            }

            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }
    }
}