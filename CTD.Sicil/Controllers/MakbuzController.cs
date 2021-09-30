using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using CTD.Core.Entities;
using CTD.Core.Helpers;
using CTD.Data.UnitofWork;
using CTD.Dto.SingleDto;
using CTD.Service.Interfaces;
using CTD.Web.Framework.Controller;
using Microsoft.Reporting.WebForms;

namespace CTD.Sicil.Controllers
{
    public class MakbuzController : AdminController
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly IMakbuzDokumService _makbuzDokumService;

        public MakbuzController(IUnitofWork uow, IKullaniciService kullaniciService,
            IMakbuzDokumService makbuzDokumService) : base(uow)
        {
            _kullaniciService = kullaniciService;
            _makbuzDokumService = makbuzDokumService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MakbuzDokum(int? sicilno, int? islem, int? sinif)
        {
            var result = _makbuzDokumService.MakbuzDokum(sicilno, islem, sinif, Accesses.Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _MakbuzDokum(int? islem, int? sinif, string siciladsoyad, string not)
        {
            if (islem != null && sinif != null)
            {
                ViewData["siciladsoyad"] = siciladsoyad;
                ViewData["not"] = not;
                var list = _makbuzDokumService.GetirTahsilatKalemleri(islem);
                foreach (var item in list)
                    item.SinifFiyat = _makbuzDokumService.GetirTahsilatKalemiFiyat(sinif, item.Id);
                return PartialView(list);
            }

            return null;
        }

        [HttpPost]
        public ActionResult MakbuzKaydet(MakbuzDokum model, int sinif)
        {
            var sonuc = "";
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            if (!string.IsNullOrEmpty(u.adi))
            {
                model.DIGERMAKBUZ = false;
                model.EVRAKMAKBUZ = false;
                model.SICILMAKBUZ = true;
                model.KAYITEDEN = Accesses.Id;
                model.KAYITTAR = DateTime.Now;
                model.SUBE = u.sube;
                _makbuzDokumService.MakbuzKaydet(model);
                _uow.SaveChanges();
                var sonId = model.Id;
                if (sonId > 0)
                {
                    var list = _makbuzDokumService.TahsilatKalemleri(model.ISLEM, sinif);
                    foreach (var item in list)
                    {
                        var md = new MakbuzDetay
                        {
                            MAKBUZDOKUMID = sonId, KOD = item.Kod, ACIKLAMA = item.TahsilatKalemi,
                            TUTAR = item.SinifFiyat, TAHAKKUKTAR = model.MAKBUZTAR, TAHAKKUKETTIREN = Accesses.Id,
                            MAKBUZ = item.Makbuz
                        };
                        _makbuzDokumService.MakbuzDetayKaydet(md);
                    }

                    _uow.SaveChanges();
                    sonuc = "Kayıt Başarılı";
                }
            }
            else
            {
                sonuc = "Hata Kodu: K-1";
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MakbuzGuncelle(MakbuzDokum model, int sinif)
        {
            var sonuc = "";
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            if (!string.IsNullOrEmpty(u.adi))
            {
                var mdokum = new MakbuzDokum();
                mdokum = _makbuzDokumService.GetirMakbuzDokum(model.Id);
                if (mdokum.ISLEM != model.ISLEM)
                {
                    var m = new MakbuzDetay();
                    var mdetay = new List<MakbuzDetay>();
                    mdetay = _makbuzDokumService.GetirMakbuzDetay(model.Id);
                    foreach (var item in mdetay)
                    {
                        m = _makbuzDokumService.GetirMakbuzDetayi(item.Id);
                        _makbuzDokumService.MakbuzDetaySil(m);
                    }

                    var makbuzdetaylarisilindi = _uow.SaveChanges();
                    if (makbuzdetaylarisilindi > 0)
                    {
                        var list = _makbuzDokumService.TahsilatKalemleri(model.ISLEM, sinif);
                        foreach (var item in list)
                        {
                            var md = new MakbuzDetay
                            {
                                MAKBUZDOKUMID = model.Id, KOD = item.Kod, ACIKLAMA = item.TahsilatKalemi,
                                TUTAR = item.SinifFiyat, TAHAKKUKTAR = model.MAKBUZTAR, TAHAKKUKETTIREN = Accesses.Id,
                                MAKBUZ = item.Makbuz
                            };
                            _makbuzDokumService.MakbuzDetayKaydet(md);
                        }

                        _uow.SaveChanges();
                        var tk = _makbuzDokumService.GetirTahsilatKalemleri(model.ISLEM);
                        foreach (var item in tk)
                            item.SinifFiyat = _makbuzDokumService.GetirTahsilatKalemiFiyat(sinif, item.Id);
                        decimal birliktahsilati = 0;
                        decimal idtahsilati = 0;
                        decimal toplamtahsilat = 0;
                        foreach (var item2 in tk)
                            if (item2.Makbuz == true)
                                birliktahsilati += item2.SinifFiyat;
                            else
                                idtahsilati += item2.SinifFiyat;
                        toplamtahsilat = birliktahsilati + idtahsilati;
                        mdokum.ADISOYADI = model.ADISOYADI;
                        mdokum.DEGEDEN = u.Id;
                        mdokum.DEGTAR = DateTime.Now;
                        mdokum.MAKBUZNO = model.MAKBUZNO;
                        mdokum.MAKBUZTAR = model.MAKBUZTAR;
                        mdokum.ODA = model.ODA;
                        mdokum.SERINO = model.SERINO;
                        mdokum.SICILNO = model.SICILNO;
                        mdokum.ISLEM = model.ISLEM;
                        mdokum.ACIKLAMA = model.ACIKLAMA;
                        mdokum.BIRLIKTAHSILATI = birliktahsilati;
                        mdokum.IDTAHSILATI = idtahsilati;
                        mdokum.TOPLAMTAHSILAT = toplamtahsilat;
                        _makbuzDokumService.MakbuzDokumGuncelle(mdokum);
                        _uow.SaveChanges();
                    }
                }
                else if (mdokum.ISLEM == model.ISLEM && mdokum.ACIKLAMA != model.ACIKLAMA)
                {
                    var listeski = _makbuzDokumService.GetirMakbuzDetay(model.Id);
                    var listyeni = _makbuzDokumService.TahsilatKalemleri(model.ISLEM, sinif);
                    foreach (var item in listeski)
                    foreach (var item2 in listyeni)
                        if (item.KOD == item2.Kod)
                        {
                            item.TUTAR = item2.SinifFiyat;
                            _makbuzDokumService.MakbuzDetayGuncelle(item);
                            _uow.SaveChanges();
                        }

                    var tk = _makbuzDokumService.GetirTahsilatKalemleri(model.ISLEM);
                    foreach (var item in tk)
                        item.SinifFiyat = _makbuzDokumService.GetirTahsilatKalemiFiyat(sinif, item.Id);
                    decimal birliktahsilati = 0;
                    decimal idtahsilati = 0;
                    decimal toplamtahsilat = 0;
                    foreach (var item2 in tk)
                        if (item2.Makbuz == true)
                            birliktahsilati += item2.SinifFiyat;
                        else
                            idtahsilati += item2.SinifFiyat;
                    toplamtahsilat = birliktahsilati + idtahsilati;
                    mdokum.ADISOYADI = model.ADISOYADI;
                    mdokum.DEGEDEN = u.Id;
                    mdokum.DEGTAR = DateTime.Now;
                    mdokum.MAKBUZNO = model.MAKBUZNO;
                    mdokum.MAKBUZTAR = model.MAKBUZTAR;
                    mdokum.ODA = model.ODA;
                    mdokum.SERINO = model.SERINO;
                    mdokum.SICILNO = model.SICILNO;
                    mdokum.ISLEM = model.ISLEM;
                    mdokum.ACIKLAMA = model.ACIKLAMA;
                    mdokum.BIRLIKTAHSILATI = birliktahsilati;
                    mdokum.IDTAHSILATI = idtahsilati;
                    mdokum.TOPLAMTAHSILAT = toplamtahsilat;
                    _makbuzDokumService.MakbuzDokumGuncelle(mdokum);
                    _uow.SaveChanges();
                }
                else if (mdokum.ISLEM == model.ISLEM && mdokum.ACIKLAMA == model.ACIKLAMA)
                {
                    mdokum.ADISOYADI = model.ADISOYADI;
                    mdokum.DEGEDEN = u.Id;
                    mdokum.DEGTAR = DateTime.Now;
                    mdokum.MAKBUZNO = model.MAKBUZNO;
                    mdokum.MAKBUZTAR = model.MAKBUZTAR;
                    mdokum.ODA = model.ODA;
                    mdokum.SERINO = model.SERINO;
                    mdokum.SICILNO = model.SICILNO;
                    _makbuzDokumService.MakbuzDokumGuncelle(mdokum);
                    _uow.SaveChanges();
                }
            }

            sonuc = "basarili";
            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakbuzSayac(string seriNo, int makbuzNo)
        {
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            if (!string.IsNullOrEmpty(u.adi))
                try
                {
                    var mevcut = _makbuzDokumService.GetirMakbuz(seriNo, u.Id);
                    mevcut.MAKBUZNO = makbuzNo;
                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    var hata = ex.Message;
                    throw;
                }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvrakSatisOda()
        {
            var e = new EvrakSatisOdaDto();
            e.MeslekOdalari = _makbuzDokumService.GetirOdalar();
            e.MakbuzBilgisi = _makbuzDokumService.GetirMakbuz(Accesses.Id);
            return View(e);
        }

        public ActionResult MakbuzOdaTahsilatKalemleri()
        {
            return Json(
                new
                {
                    MeslekOdalari = _makbuzDokumService.GetirOdalar(),
                    TahsilatKalemleri = _makbuzDokumService.GetirTahsilatKalemleri("Evrak Satış İşlemi")
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakbuzDokumLogKaydet(MakbuzDokumLogDto model)
        {
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            var mdl = new MakbuzDokumLog
            {
                ODA = model.Oda, SERINO = model.SeriNo, MAKBUZNO = model.MakbuzNo,
                MAKBUZTAR = DateHelper.GetDateTimeCultural(model.MakbuzTarihi), ADISOYADI = model.AdiSoyadi, DIGERMAKBUZ = false,
                EVRAKMAKBUZ = true, ISLEMIYAPAN = Accesses.Id, SUBE = u.sube, ISLEM = 12, KAYITEDEN = Accesses.Id,
                KAYITTAR = DateHelper.GetDateTimeCultural(model.MakbuzTarihi), SICILMAKBUZ = false, SICILNO = model.SicilNo
            };
            _makbuzDokumService.MakbuzDokumLogKaydet(mdl);
            _uow.SaveChanges();
            var result = mdl.Id;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakbuzDokumLogKaydet2(MakbuzDokumLogDto model)
        {
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            var mdl = new MakbuzDokumLog
            {
                DIGERMAKBUZ = true, EVRAKMAKBUZ = false, ISLEMIYAPAN = Accesses.Id, SUBE = u.sube,
                KAYITEDEN = Accesses.Id, MAKBUZTAR = DateHelper.GetDateTimeCultural(model.MakbuzTarihi),
                KAYITTAR = DateHelper.GetDateTimeCultural(model.MakbuzTarihi), SICILMAKBUZ = false, SERINO = model.SeriNo,
                ODA = model.Oda, MAKBUZNO = model.MakbuzNo, ADISOYADI = model.AdiSoyadi, ISLEM = 14
            };
            _makbuzDokumService.MakbuzDokumLogKaydet(mdl);
            _uow.SaveChanges();
            var result = mdl.Id;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakbuzDetayLogKaydet(MakbuzDetayLogDto model)
        {
            var makbuzdetaylog = new MakbuzDetayLog
            {
                TAHAKKUKETTIREN = Accesses.Id, MAKBUZ = true, MAKBUZDOKUMID = model.MakbuzDokumId, KOD = model.Kod,
                ACIKLAMA = model.Aciklama, TUTAR = model.Tutar, TAHAKKUKTAR = DateHelper.GetDateTimeCultural(model.TahakkukTarihi)
            };
            _makbuzDokumService.MakbuzDetayLogKaydet(makbuzdetaylog);
            _uow.SaveChanges();
            return Json("basarili", JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _EvrakDokum(int? makbuzdokumid)
        {
            if (makbuzdokumid != null)
            {
                ViewBag.MakbuzDokumId = makbuzdokumid;
                var list = _makbuzDokumService.GetirMakbuzDetayLogList(makbuzdokumid);
                return PartialView(list);
            }

            return null;
        }

        public JsonResult MakbuzDetayLogSil(int id)
        {
            _makbuzDokumService.MakbuzDetayLogSil(id);
            _uow.SaveChanges();
            return Json("silindi", JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakbuzDokumLogGuncelle(int makbuzdokumid, string aciklama, string toplamtutar)
        {
            var t = 0;
            try
            {
                var mevcut = _makbuzDokumService.GetirMakbuzDokumLog(makbuzdokumid);
                mevcut.ACIKLAMA = aciklama;
                mevcut.BIRLIKTAHSILATI = Convert.ToDecimal(toplamtutar);
                mevcut.TOPLAMTAHSILAT = Convert.ToDecimal(toplamtutar);
                mevcut.IDTAHSILATI = 0;
                var s = _uow.SaveChanges();
                if (s == 1) t = _makbuzDokumService.MakbuzDokumKayitiniAktar(makbuzdokumid);
            }
            catch (Exception ex)
            {
                var hata = ex.Message;
                throw;
            }

            return Json(t, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakbuzDokumLogGuncelle2(int makbuzdokumid, int yenimakbuzdokumid)
        {
            _makbuzDokumService.MakbuzDetayKayitlariniAktar(makbuzdokumid, yenimakbuzdokumid);
            try
            {
                var makbuzdetayloglar = _makbuzDokumService.GetirSilinecekMakbuzDetayLoglari(makbuzdokumid);
                foreach (var item in makbuzdetayloglar)
                {
                    var m = new MakbuzDetayLog();
                    m = _makbuzDokumService.GetirMakbuzDetayLog(item.Id);
                    _makbuzDokumService.SilMakbuzDetayLog(m);
                }

                var mdlsilindi = _uow.SaveChanges();
                if (mdlsilindi > 0)
                {
                    var md = new MakbuzDokumLog();
                    md = _makbuzDokumService.GetirMakbuzDokumLog(makbuzdokumid);
                    _makbuzDokumService.SilMakbuzDokumLog(md);
                    _uow.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var hata = ex.Message;
                throw;
            }

            return Json(yenimakbuzdokumid, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _PrintEvrakMakbuzOda(int? makbuzdokumid, int? oda, string aciklama)
        {
            if (makbuzdokumid != null)
            {
                var pmm = new PrintMakbuzDto();
                pmm.MakbuzKalemleri = _makbuzDokumService.GetirMakbuzDetay(makbuzdokumid);
                pmm.MakbuzAciklamasi = aciklama;
                pmm.OdaTamAdi = _makbuzDokumService.GetirMeslekOdasi(oda);
                return PartialView(pmm);
            }

            return null;
        }

        public ActionResult EvrakSatisEsnaf()
        {
            var e = new EvrakSatisEsnafDto();
            e.MakbuzBilgisi = _makbuzDokumService.GetirMakbuz(Accesses.Id);
            e.TahsilatIslemleri = _makbuzDokumService.GetirTahsilatKalemleri("Evrak Satış İşlemi");
            return View(e);
        }

        public PartialViewResult _PrintEvrakMakbuzEsnaf(int? makbuzdokumid, int? sicilno, string adsoyad,
            string aciklama)
        {
            if (makbuzdokumid != null)
            {
                var pmm = new PrintMakbuzDto();
                pmm.MakbuzKalemleri = _makbuzDokumService.GetirMakbuzDetay(makbuzdokumid);
                pmm.MakbuzAciklamasi = aciklama;
                pmm.AdSoyad = adsoyad;
                pmm.SicilNo = sicilno;
                return PartialView(pmm);
            }

            return null;
        }

        public ActionResult EgitimKatilim()
        {
            var e = new EgitimKatilimDto();
            e.MakbuzBilgisi = _makbuzDokumService.GetirMakbuz(Accesses.Id);
            e.MeslekOdalari = _makbuzDokumService.GetirOdalar();
            e.TahsilatIslemleri = _makbuzDokumService.GetirTahsilatKalemleri("Eğitim-Katılım");
            return View(e);
        }

        public ActionResult EgitimKatilimSabitleri()
        {
            return Json(
                new
                {
                    MeslekOdalari = _makbuzDokumService.GetirOdalar(),
                    TahsilatIslemleri = _makbuzDokumService.GetirTahsilatKalemleri("Eğitim-Katılım")
                }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _EgitimKatilimDokum(int? makbuzdokumid)
        {
            if (makbuzdokumid != null)
            {
                ViewBag.MakbuzDokumId = makbuzdokumid;
                var list = _makbuzDokumService.GetirMakbuzDetayLogList(makbuzdokumid);
                return PartialView(list);
            }

            return null;
        }

        public PartialViewResult _PrintEKMakbuz(int? makbuzdokumid, int? oda, string aciklama)
        {
            if (makbuzdokumid != null)
            {
                var pmm = new PrintMakbuzDto();
                pmm.MakbuzKalemleri = _makbuzDokumService.GetirMakbuzDetay(makbuzdokumid);
                pmm.MakbuzAciklamasi = aciklama;
                pmm.OdaTamAdi = _makbuzDokumService.GetirMeslekOdasi(oda);
                return PartialView(pmm);
            }

            return null;
        }

        public ActionResult Bagis()
        {
            var e = new EgitimKatilimDto();
            e.MakbuzBilgisi = _makbuzDokumService.GetirMakbuz(Accesses.Id);
            return View(e);
        }

        public JsonResult BagisEkle(string serino, int makbuzno, string makbuztarihi, string bagisyapan, decimal tutar)
        {
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            var mki = 0;
            try
            {
                var md = new MakbuzDokum
                {
                    SICILMAKBUZ = false, EVRAKMAKBUZ = false, DIGERMAKBUZ = true,
                    TOPLAMTAHSILAT = Convert.ToDecimal(tutar), BIRLIKTAHSILATI = Convert.ToDecimal(tutar),
                    IDTAHSILATI = 0, SERINO = serino, MAKBUZNO = makbuzno, MAKBUZTAR = DateHelper.GetDateTimeCultural(makbuztarihi),
                    ACIKLAMA = bagisyapan, KAYITEDEN = Accesses.Id, ISLEMIYAPAN = Accesses.Id, SUBE = u.sube,
                    ADISOYADI = bagisyapan, KAYITTAR = DateTime.Now, ISLEM = 15
                };
                _makbuzDokumService.BagisMakbuzDokumEkle(md);
                var sonuc = _uow.SaveChanges();
                if (sonuc > 0) mki = md.Id;
            }
            catch (Exception ex)
            {
                var hata = ex.Message;
                throw;
            }

            if (mki > 0)
                try
                {
                    var mdetay = new MakbuzDetay
                    {
                        MAKBUZDOKUMID = mki, KOD = "BY", ACIKLAMA = "BAĞIŞ VE YARDIMLAR",
                        TUTAR = Convert.ToDecimal(tutar), TAHAKKUKTAR = DateHelper.GetDateTimeCultural(makbuztarihi),
                        TAHAKKUKETTIREN = Accesses.Id, MAKBUZ = true
                    };
                    _makbuzDokumService.BagisMakbuzDetayEkle(mdetay);
                    var s = _uow.SaveChanges();
                    if (s > 0)
                        try
                        {
                            MakbuzSayac(serino, makbuzno);
                        }
                        catch (Exception ex)
                        {
                            var hata = ex.Message;
                            throw;
                        }
                }
                catch (Exception ex)
                {
                    var hata = ex.Message;
                    throw;
                }

            return Json(mki, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _PrintBMakbuz(int? makbuzdokumid, string bagisyapan)
        {
            if (makbuzdokumid != null)
            {
                var pmm = new PrintMakbuzDto();
                pmm.MakbuzKalemleri = _makbuzDokumService.GetirMakbuzDetay(makbuzdokumid);
                pmm.MakbuzAciklamasi = bagisyapan;
                return PartialView(pmm);
            }

            return null;
        }

        public ActionResult Kira()
        {
            var e = new EgitimKatilimDto();
            e.MakbuzBilgisi = _makbuzDokumService.GetirMakbuz(Accesses.Id);
            return View(e);
        }

        public PartialViewResult _PrintKiraMakbuz(int? makbuzdokumid, string aciklama)
        {
            if (makbuzdokumid != null)
            {
                var pmm = new PrintMakbuzDto();
                pmm.MakbuzKalemleri = _makbuzDokumService.GetirMakbuzDetay(makbuzdokumid);
                pmm.MakbuzAciklamasi = aciklama;
                return PartialView(pmm);
            }

            return null;
        }

        public JsonResult KiraEkle(string serino, int makbuzno, string makbuztarihi, string kiraci, decimal tutar,
            string aciklama)
        {
            var u = _kullaniciService.KullaniciGetir(Accesses.Id);
            var mki = 0;
            try
            {
                var md = new MakbuzDokum
                {
                    SICILMAKBUZ = false, EVRAKMAKBUZ = false, DIGERMAKBUZ = true,
                    TOPLAMTAHSILAT = Convert.ToDecimal(tutar), BIRLIKTAHSILATI = Convert.ToDecimal(tutar),
                    IDTAHSILATI = 0, SERINO = serino, MAKBUZNO = makbuzno, MAKBUZTAR = DateHelper.GetDateTimeCultural(makbuztarihi),
                    ACIKLAMA = aciklama, KAYITEDEN = Accesses.Id, ISLEMIYAPAN = Accesses.Id, SUBE = u.sube,
                    ADISOYADI = kiraci, KAYITTAR = DateTime.Now, ISLEM = 1016
                };
                _makbuzDokumService.KiraMakbuzDokumEkle(md);
                var sonuc = _uow.SaveChanges();
                if (sonuc > 0) mki = md.Id;
            }
            catch (Exception ex)
            {
                var hata = ex.Message;
                throw;
            }

            if (mki > 0)
                try
                {
                    var mdetay = new MakbuzDetay
                    {
                        MAKBUZDOKUMID = mki, KOD = "BY", ACIKLAMA = "KİRA GELİRLERİ", TUTAR = Convert.ToDecimal(tutar),
                        TAHAKKUKTAR = DateHelper.GetDateTimeCultural(makbuztarihi), TAHAKKUKETTIREN = Accesses.Id, MAKBUZ = true
                    };
                    _makbuzDokumService.KiraMakbuzDetayEkle(mdetay);
                    var s = _uow.SaveChanges();
                    if (s > 0)
                        try
                        {
                            MakbuzSayac(serino, makbuzno);
                        }
                        catch (Exception ex)
                        {
                            var hata = ex.Message;
                            throw;
                        }
                }
                catch (Exception ex)
                {
                    var hata = ex.Message;
                    throw;
                }

            return Json(mki, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakbuzIptal()
        {
            return View();
        }

        public ActionResult _MakbuzDetay(string serino, int makbuzno, string makbuztarihi)
        {
            if (serino != null && makbuzno != 0 && makbuztarihi != null)
            {
                var dt = new DateTime();
                dt = DateHelper.GetDateTimeCultural(makbuztarihi);
                var md = new MakbuzDokum();
                md = _makbuzDokumService.GetirMakbuzDokum(serino, makbuzno, Accesses.Id, dt);
                return Json(md, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult MakbuzIptalEt(int makbuzid, string makbuztarihi)
        {
            var sonuc = "";
            var m = new MakbuzDetay();
            var mdetay = new List<MakbuzDetay>();
            mdetay = _makbuzDokumService.GetirMakbuzDetay(makbuzid);
            foreach (var item in mdetay)
            {
                m = _makbuzDokumService.GetirMakbuzDetayi(item.Id);
                _makbuzDokumService.MakbuzDetaySil(m);
            }

            var makbuzdetaylarisilindi = _uow.SaveChanges();
            if (makbuzdetaylarisilindi > 0)
            {
                var yenimakbuzdetayeklendi = 0;
                try
                {
                    m.MAKBUZDOKUMID = makbuzid;
                    m.KOD = "İPTAL";
                    m.ACIKLAMA = "İPTAL MAKBUZU";
                    m.TUTAR = 0;
                    m.TAHAKKUKTAR = DateHelper.GetDateTimeCultural(makbuztarihi);
                    m.TAHAKKUKETTIREN = Accesses.Id;
                    m.MAKBUZ = false;
                    _makbuzDokumService.MakbuzDetayKaydet(m);
                    yenimakbuzdetayeklendi = _uow.SaveChanges();
                }
                catch (Exception)
                {
                    sonuc = "Hata Kodu: M-49";
                }

                if (yenimakbuzdetayeklendi == 1)
                {
                    var mdokum = new MakbuzDokum();
                    mdokum = _makbuzDokumService.GetirMakbuzDokum(makbuzid);
                    if (mdokum != null)
                        try
                        {
                            mdokum.TOPLAMTAHSILAT = 0;
                            mdokum.BIRLIKTAHSILATI = 0;
                            mdokum.IDTAHSILATI = 0;
                            mdokum.ACIKLAMA = "İPTAL";
                            mdokum.IPTAL = true;
                            mdokum.IPTALTAR = DateTime.Now;
                            mdokum.IPTALEDEN = Accesses.Id;
                            _uow.SaveChanges();
                            sonuc = "ok";
                        }
                        catch (Exception)
                        {
                            sonuc = "Hata Kodu: M-48";
                        }
                }
                else
                {
                    sonuc = "Hata Kodu: M-49";
                }
            }
            else
            {
                sonuc = "Hata Kodu: M-50";
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakbuzSil()
        {
            return View();
        }

        public ActionResult _MakbuzSilmeList(string serino, int makbuzno, string makbuztarihi)
        {
            if (serino != null && makbuzno != 0 && makbuztarihi != null)
            {
                var makbuztar = DateHelper.GetDateTimeCultural(makbuztarihi);
                var model = _makbuzDokumService.GetirSilinecekMakbuzlar(serino, makbuzno, makbuztar);
                var ds = "<table id='tblSilinecekMakbuzlar' class='display' cellspacing='0' width='100%'>" +
                         Environment.NewLine +
                         "<thead><tr><th>#</th><th>SERİ NO</th><th>MAKBUZ NO</th><th>KAYIT TARİHİ</th><th>ADI SOYADI</th><th>AÇIKLAMA</th><th></th></tr></thead><tbody>";
                foreach (var item in model)
                    ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.SeriNo + "</td><td>" +
                          item.MakbuzNo + "</td><td>" + item.KayitTarihi + "</td><td>" + item.AdiSoyadi + "</td><td>" +
                          item.Aciklama + "</td><td><a href='#' id='" + item.Id + "' onclick='return MakbuzSil(" +
                          item.Id + ");' title='Sil' data-id='" + item.Id +
                          "'><i class='fa fa-trash fa-2x'></i></a></td></tr>";
                ds += "</tbody></table>";
                return Json(new {ds}, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult MakbuzKaydiSil(int makbuzid)
        {
            var sonuc = "";
            var m = new MakbuzDetay();
            var mdetay = new List<MakbuzDetay>();
            mdetay = _makbuzDokumService.GetirMakbuzDetay(makbuzid);
            foreach (var item in mdetay)
            {
                m = _makbuzDokumService.GetirMakbuzDetayi(item.Id);
                _makbuzDokumService.MakbuzDetaySil(m);
            }

            var makbuzdetaylarisilindi = _uow.SaveChanges();
            if (makbuzdetaylarisilindi > 0)
            {
                _makbuzDokumService.MakbuzDokumSil(makbuzid);
                _uow.SaveChanges();
                sonuc = "ok";
            }
            else
            {
                sonuc = "Hata Kodu: M-50";
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VezneTahsilatListesi()
        {
            return View();
        }

        public ActionResult TahsilatListesi()
        {
            return View();
        }

        public ActionResult GetirVezneler()
        {
            return Json(new {VezneListesi = _makbuzDokumService.GetirVezneler(Accesses.Id)},
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult _TahsilatTuruneGore(string ilktarih, string sontarih, int vezne)
        {
            if (ilktarih != null && sontarih != null && vezne != 0)
            {
                var it = DateHelper.GetDateTimeCultural(ilktarih);
                var st = DateHelper.GetDateTimeCultural(sontarih);
                var data = _makbuzDokumService.TahsilatTuruneGoreListe(it, st, vezne);
                return PartialView(data);
            }

            return null;
        }

        public ActionResult Report(string id, string ilktarih, string sontarih, string vezne)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpTahsilatListesi.rdlc");
            if (System.IO.File.Exists(path))
                lr.ReportPath = path;
            else
                return null;
            var it = DateHelper.GetDateTimeCultural(ilktarih);
            var st = DateHelper.GetDateTimeCultural(sontarih);
            var kullanici = int.Parse(vezne);
            var birlikMakbuz = _makbuzDokumService.TahsilatTuruneGoreListe2(it, st, kullanici);
            var lstBirlik = new List<MakbuzDokum>();
            foreach (var item in birlikMakbuz)
            {
                var mdl = new MakbuzDokum
                {
                    ACIKLAMA = item.ACIKLAMA, SERINO = item.SERINO, MAKBUZNO = item.MAKBUZNO,
                    MAKBUZTAR = item.MAKBUZTAR, BIRLIKTAHSILATI = item.BIRLIKTAHSILATI, IPTAL = item.IPTAL,
                    SICILNO = item.SICILNO, ADISOYADI = item.adsoyad
                };
                lstBirlik.Add(mdl);
            }

            var idMakbuz = _makbuzDokumService.TahsilatTuruneGoreIDListe(it, st, kullanici);
            var lstId = new List<MakbuzDokum>();
            foreach (var item in idMakbuz)
            {
                var mdl = new MakbuzDokum
                {
                    ACIKLAMA = item.ACIKLAMA, SERINO = item.SERINO, MAKBUZNO = item.MAKBUZNO,
                    MAKBUZTAR = item.MAKBUZTAR, IDTAHSILATI = item.IDTAHSILATI, IPTAL = item.IPTAL,
                    SICILNO = item.SICILNO, ADISOYADI = item.adsoyad
                };
                lstId.Add(mdl);
            }

            lr.DataSources.Clear();
            var rdBirlik = new ReportDataSource("BirlikTahsilati", lstBirlik);
            var rdId = new ReportDataSource("IDTahsilati", lstId);
            var p1 = new ReportParameter("ilktarih", ilktarih);
            var p2 = new ReportParameter("sontarih", sontarih);
            lr.SetParameters(new[] {p1, p2});
            lr.DataSources.Add(rdBirlik);
            lr.DataSources.Add(rdId);
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

        public ActionResult ReportV2(string id, string ilktarih, string sontarih, string vezne)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpTahsilatListesiV2.rdlc");
            if (System.IO.File.Exists(path))
                lr.ReportPath = path;
            else
                return null;
            var it = DateHelper.GetDateTimeCultural(ilktarih);
            var st = DateHelper.GetDateTimeCultural(sontarih);
            var kullanici = int.Parse(vezne);
            var Makbuz = _makbuzDokumService.TahsilatTuruneGoreListeV2(it, st, kullanici);
            var lstMakbuz = new List<MakbuzDokum>();
            foreach (var item in Makbuz)
            {
                var mdl = new MakbuzDokum
                {
                    ACIKLAMA = item.ACIKLAMA, SERINO = item.SERINO, MAKBUZNO = item.MAKBUZNO,
                    MAKBUZTAR = item.MAKBUZTAR, BIRLIKTAHSILATI = item.BIRLIKTAHSILATI, IDTAHSILATI = item.IDTAHSILATI,
                    IPTAL = item.IPTAL, SICILNO = item.SICILNO, ADISOYADI = item.adsoyad
                };
                lstMakbuz.Add(mdl);
            }

            lr.DataSources.Clear();
            var rdMakbuz = new ReportDataSource("TahsilatTuruneGoreListeV2", lstMakbuz);
            var p1 = new ReportParameter("ilktarih", ilktarih);
            var p2 = new ReportParameter("sontarih", sontarih);
            lr.SetParameters(new[] {p1, p2});
            lr.DataSources.Add(rdMakbuz);
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

        public ActionResult GelirTablosu()
        {
            return View();
        }

        public ActionResult _MakbuzGelirTablosu(string ilktarih, string sontarih, int vezne)
        {
            if (ilktarih != null && sontarih != null)
            {
                var it = DateHelper.GetDateTimeCultural(ilktarih);
                var st = DateHelper.GetDateTimeCultural(sontarih);
                var data = _makbuzDokumService.MakbuzGelirTablosu(it, st, vezne);
                return PartialView(data);
            }

            return null;
        }

        public ActionResult ReportGelirTablosu(string id, string ilktarih, string sontarih, string vezne)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpMakbuzGelirTablosu.rdlc");
            if (System.IO.File.Exists(path))
                lr.ReportPath = path;
            else
                return null;
            var it = DateHelper.GetDateTimeCultural(ilktarih);
            var st = DateHelper.GetDateTimeCultural(sontarih);
            var kullanici = int.Parse(vezne);
            var kullaniciadi = _makbuzDokumService.GetirKullanici(kullanici);
            var data = _makbuzDokumService.MakbuzGelirTablosu(it, st, kullanici);
            var makbuzsayisi =
                _makbuzDokumService.MakbuzGelirTablosuMakbuzSayisi(it, st,
                    kullanici); /*_db.spMakbuzGelirTablosuMakbuzSayisi(it, st, Accesses.Id).SingleOrDefault();*/
            var toplambirliktahsilati =
                _makbuzDokumService.ToplamBirlikTahsilati(it, st,
                    kullanici); /*_db.spToplamBirlikTahsilati(it, st, Accesses.Id).SingleOrDefault();*/
            lr.DataSources.Clear();
            var rd = new ReportDataSource("DSReports", data);
            var p1 = new ReportParameter("ilktarih", ilktarih);
            var p2 = new ReportParameter("sontarih", sontarih);
            var p3 = new ReportParameter("kullaniciadi", kullaniciadi);
            var p4 = new ReportParameter("makbuzadedi", makbuzsayisi.ToString());
            var p5 = new ReportParameter("toplambirliktahsilati", toplambirliktahsilati.ToString());
            lr.SetParameters(new[] {p1, p2, p3, p4, p5});
            lr.DataSources.Add(rd);
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

        public ActionResult EskiMakbuz(int? id)
        {
            if (id != null)
            {
                var model = new MakbuzTekrar();
                model.MakbuzDokum = _makbuzDokumService.GetirMakbuzDokum(id);
                model.MakbuzDetaylari = _makbuzDokumService.GetirMakbuzDetay(id);
                return View(model);
            }

            return View();
        }

        [HttpPost]
        public ActionResult EskiMakbuzGetir2(EskiMakbuzGetirDto model)
        {
            var m = _makbuzDokumService.MakbuzKontrol(model.SeriNo, model.MakbuzNo,
                DateHelper.GetDateTimeCultural(model.MakbuzTarihi?.ToString()));
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakbuzYazdir(int id)
        {
            var model = new MakbuzTekrar();
            model.MakbuzDokum = _makbuzDokumService.GetirMakbuzDokum(id);
            model.MakbuzDetaylari = _makbuzDokumService.GetirMakbuzDetay(id);
            return View(model);
        }

        public ActionResult ReportEskiMakbuz(int id)
        {
            var lr = new LocalReport();
            var path = Path.Combine(Server.MapPath("~/Reports"), "rpMakbuz-A4.rdlc");
            if (System.IO.File.Exists(path))
                lr.ReportPath = path;
            else
                return null;
            var MakbuzDokum = _makbuzDokumService.GetirMakbuzDokum(id);
            var MakbuzDetaylari = _makbuzDokumService.GetirMakbuzDetay(id);
            var makbuzdetay = new List<MakbuzDetay>();
            foreach (var item in MakbuzDetaylari)
                if (item.MAKBUZ == true)
                {
                    var md = new MakbuzDetay {ACIKLAMA = item.ACIKLAMA, TUTAR = item.TUTAR};
                    makbuzdetay.Add(md);
                }

            lr.DataSources.Clear();
            var rdMakbuzDetay = new ReportDataSource("MakbuzDetay", makbuzdetay);
            ReportParameter prAdSoyad, prMakbuzTarihi, prMakbuzAciklama, prToplamTutar, prToplamTutarYazi;
            if (MakbuzDokum.SICILMAKBUZ == true)
            {
                prAdSoyad = new ReportParameter("prAdSoyad", MakbuzDokum.ADISOYADI);
                prMakbuzTarihi = new ReportParameter("prMakbuzTarihi", MakbuzDokum.MAKBUZTAR.ToStringTR());
                prMakbuzAciklama = new ReportParameter("prMakbuzAciklama", MakbuzDokum.ACIKLAMA);
                prToplamTutar = new ReportParameter("prToplamTutar", MakbuzDokum.BIRLIKTAHSILATI.ToString());
                prToplamTutarYazi =
                    new ReportParameter("prToplamTutarYazi", DecimalToString(MakbuzDokum.BIRLIKTAHSILATI));
                lr.SetParameters(new[] {prAdSoyad, prMakbuzTarihi, prMakbuzAciklama, prToplamTutar, prToplamTutarYazi});
            }
            else if (MakbuzDokum.EVRAKMAKBUZ == true)
            {
                if (MakbuzDokum.ODA != null && MakbuzDokum.SICILNO == null)
                {
                    prAdSoyad = new ReportParameter("prAdSoyad", _makbuzDokumService.GetirMeslekOdasi(MakbuzDokum.ODA));
                    prMakbuzTarihi = new ReportParameter("prMakbuzTarihi", MakbuzDokum.MAKBUZTAR.ToStringTR());
                    prMakbuzAciklama = new ReportParameter("prMakbuzAciklama", MakbuzDokum.ACIKLAMA);
                    prToplamTutar = new ReportParameter("prToplamTutar", MakbuzDokum.BIRLIKTAHSILATI.ToString());
                    prToplamTutarYazi =
                        new ReportParameter("prToplamTutarYazi", DecimalToString(MakbuzDokum.BIRLIKTAHSILATI));
                }
                else if (MakbuzDokum.ODA == null && MakbuzDokum.SICILNO != null)
                {
                    prAdSoyad = new ReportParameter("prAdSoyad", MakbuzDokum.ADISOYADI);
                    prMakbuzTarihi = new ReportParameter("prMakbuzTarihi", MakbuzDokum.MAKBUZTAR.ToStringTR());
                    prMakbuzAciklama = new ReportParameter("prMakbuzAciklama", MakbuzDokum.ACIKLAMA);
                    prToplamTutar = new ReportParameter("prToplamTutar", MakbuzDokum.BIRLIKTAHSILATI.ToString());
                    prToplamTutarYazi =
                        new ReportParameter("prToplamTutarYazi", DecimalToString(MakbuzDokum.BIRLIKTAHSILATI));
                }
                else
                {
                    prAdSoyad = new ReportParameter("prAdSoyad", MakbuzDokum.ADISOYADI);
                    prMakbuzTarihi = new ReportParameter("prMakbuzTarihi", MakbuzDokum.MAKBUZTAR.ToStringTR());
                    prMakbuzAciklama = new ReportParameter("prMakbuzAciklama", MakbuzDokum.ACIKLAMA);
                    prToplamTutar = new ReportParameter("prToplamTutar", MakbuzDokum.BIRLIKTAHSILATI.ToString());
                    prToplamTutarYazi =
                        new ReportParameter("prToplamTutarYazi", DecimalToString(MakbuzDokum.BIRLIKTAHSILATI));
                }

                lr.SetParameters(new[] {prAdSoyad, prMakbuzTarihi, prMakbuzAciklama, prToplamTutar, prToplamTutarYazi});
            }
            else if (MakbuzDokum.DIGERMAKBUZ == true)
            {
                if (MakbuzDokum.ODA != null && MakbuzDokum.SICILNO == null)
                {
                    prAdSoyad = new ReportParameter("prAdSoyad", _makbuzDokumService.GetirMeslekOdasi(MakbuzDokum.ODA));
                    prMakbuzTarihi = new ReportParameter("prMakbuzTarihi", MakbuzDokum.MAKBUZTAR.ToStringTR());
                    prMakbuzAciklama = new ReportParameter("prMakbuzAciklama", MakbuzDokum.ACIKLAMA);
                    prToplamTutar = new ReportParameter("prToplamTutar", MakbuzDokum.BIRLIKTAHSILATI.ToString());
                    prToplamTutarYazi =
                        new ReportParameter("prToplamTutarYazi", DecimalToString(MakbuzDokum.BIRLIKTAHSILATI));
                    lr.SetParameters(new[]
                        {prAdSoyad, prMakbuzTarihi, prMakbuzAciklama, prToplamTutar, prToplamTutarYazi});
                }
                else if (MakbuzDokum.ODA == null && MakbuzDokum.SICILNO == null)
                {
                    prAdSoyad = new ReportParameter("prAdSoyad", MakbuzDokum.ADISOYADI);
                    prMakbuzTarihi = new ReportParameter("prMakbuzTarihi", MakbuzDokum.MAKBUZTAR.ToStringTR());
                    prMakbuzAciklama = new ReportParameter("prMakbuzAciklama", MakbuzDokum.ACIKLAMA);
                    prToplamTutar = new ReportParameter("prToplamTutar", MakbuzDokum.BIRLIKTAHSILATI.ToString());
                    prToplamTutarYazi =
                        new ReportParameter("prToplamTutarYazi", DecimalToString(MakbuzDokum.BIRLIKTAHSILATI));
                    lr.SetParameters(new[]
                        {prAdSoyad, prMakbuzTarihi, prMakbuzAciklama, prToplamTutar, prToplamTutarYazi});
                }
            }

            lr.DataSources.Add(rdMakbuzDetay);
            lr.Refresh();
            var reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            var deviceInfo = "<DeviceInfo>" + "<OutputFormat>" + reportType + "</OutputFormat>" +
                             "<PageWidth>8.5in</PageWidth>" + "<PageHeight>11in</PageHeight>" +
                             "<MarginTop>0.1in</MarginTop>" + "<MarginLeft>0.1in</MarginLeft>" +
                             "<MarginRight>0.1in</MarginRight>" + "<MarginBottom>0.1in</MarginBottom>" +
                             "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);
            return File(renderedBytes, mimeType);
        }

        public ActionResult MakbuzDuzelt()
        {
            return View();
        }

        private string DecimalToString(decimal? tutar)
        {
            var salePrice = tutar;
            var sTutar = salePrice.ToString().Replace('.', ',');
            var lira = sTutar.Substring(0, sTutar.IndexOf(','));
            var kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
            var yazi = "";
            string[] birler = {"", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz"};
            string[] onlar = {"", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan"};
            string[] binler = {"katrilyon", "trilyon", "milyar", "milyon", "bin", ""};
            var grupSayisi = 6;
            lira = lira.PadLeft(grupSayisi * 3, '0');
            string grupDegeri;
            for (var j = 0; j < grupSayisi * 3; j += 3)
            {
                grupDegeri = "";
                if (lira.Substring(j, 1) != "0") grupDegeri += birler[Convert.ToInt32(lira.Substring(j, 1))] + "yüz";
                if (grupDegeri == "biryüz") grupDegeri = "yüz";
                grupDegeri += onlar[Convert.ToInt32(lira.Substring(j + 1, 1))];
                grupDegeri += birler[Convert.ToInt32(lira.Substring(j + 2, 1))];
                if (grupDegeri != "") grupDegeri += binler[j / 3];
                if (grupDegeri == "birbin") grupDegeri = "bin";
                yazi += grupDegeri;
            }

            if (yazi != "") yazi += " TL ";
            var yaziUzunlugu = yazi.Length;
            if (kurus.Substring(0, 1) != "0") yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];
            if (kurus.Substring(1, 1) != "0") yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];
            if (yazi.Length > yaziUzunlugu) yazi += "kuruş";
            return yazi;
        }
    }
}