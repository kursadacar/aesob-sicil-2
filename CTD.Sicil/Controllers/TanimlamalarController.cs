﻿using System;
using System.Web.Mvc;
using CTD.Core.Entities;
using CTD.Data.UnitofWork;
using CTD.Service.Interfaces;
using CTD.Web.Framework.Controller;

namespace CTD.Sicil.Controllers
{
    public class TanimlamalarController : PublicController
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly ITanimService _tanimService;

        public TanimlamalarController(IUnitofWork uow, IKullaniciService kullaniciService, ITanimService tanimService) :
            base(uow)
        {
            _kullaniciService = kullaniciService;
            _tanimService = tanimService;
        }

        public ActionResult IPTanimlama()
        {
            return View();
        }

        public ActionResult MeslekOdasiTanimlama()
        {
            return View();
        }

        public ActionResult MeslekTerkNedeni()
        {
            return View();
        }

        public ActionResult MahalleTanimlama()
        {
            return View();
        }

        public ActionResult CadSokBulvTanimlama()
        {
            return View();
        }

        public ActionResult MeslekveNaceTanimlama()
        {
            return View();
        }

        public ActionResult ComboBoxKullanicilar()
        {
            return Json(new {KullaniciListesi = _kullaniciService.ComboBoxKullanicilar()},
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult KullaniciIp(int id)
        {
            var u = _kullaniciService.KullaniciGetir(id);
            if (u != null)
            {
                var ip = u.ip;
                var gorevyeri = u.hak;
                var ilce = u.sube;
                return Json(new {ip, gorevyeri, ilce}, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public JsonResult KullaniciIpKaydet(int id, string ip, string hak, string sube)
        {
            var model = _kullaniciService.KullaniciGetir(id);
            if (model != null)
            {
                model.Id = id;
                model.ip = ip;
                model.hak = hak;
                model.sube = sube;
                _kullaniciService.IPGuncelle(model);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult OdalariGetir()
        {
            var unvan = _tanimService.GetirOdalar();
            var ds = "<table id='tblOdalar' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>KISA AD</th><th>TAM ADI</th><th>DURUM</th><th></th></tr></thead><tbody>";
            foreach (var item in unvan)
                if (item.DURUM)
                    ds += "<tr href='http://myspace.com'><td>" + item.KISAAD + "</td><td>" + item.ACIKLAMA +
                          "</td><td>Aktif</td><td><a href='#' id='" + item.Id +
                          "' data-toggle='modal' data-target='#modalEdit' onclick='return MeslekOdasiDuzenle(" +
                          item.Id + ");' title='Düzenle' data-id='" + item.Id +
                          "'><i class='fa fa-edit'></i></a></td></tr>";
                else
                    ds += "<tr href='http://myspace.com'><td>" + item.KISAAD + "</td><td>" + item.ACIKLAMA +
                          "</td><td>Pasif</td><td></td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MeslekOdasiDuzenle(int id)
        {
            var m = _tanimService.GetirOda(id);
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MeslekOdasiKaydet(MeslekOdasi model)
        {
            var mo = _tanimService.GetirOda(model.Id);
            if (model != null)
            {
                mo.KISAAD = model.KISAAD;
                mo.ACIKLAMA = model.ACIKLAMA;
                mo.SINIF = model.SINIF;
                mo.DURUM = model.DURUM;
                _tanimService.MeslekOdasiGuncelle(mo);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult GetirTerkNedenleri()
        {
            var model = _tanimService.GetirTerkNedenleri();
            var ds = "<table id='tblTerkNedenleri' class='display' cellspacing='0' width='100%'>" +
                     Environment.NewLine + "<thead><tr><th>#</th><th>TERK NEDENİ</th><th></th></tr></thead><tbody>";
            foreach (var item in model)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.ACIKLAMA +
                      "</td><td><a href='#' id='" + item.Id + "' onclick='return TerkNedeniDuzenle(" + item.Id +
                      ");' title='Düzenle' data-id='" + item.Id + "'><i class='fa fa-edit'></i></a></td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetirTerkNedeni(int id)
        {
            var m = _tanimService.GetirTerkNedeni(id);
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TerkNedenikaydet(MeslekTerkNedeni model)
        {
            MeslekTerkNedeni sm = null;
            sm = model.Id > 0 ? _tanimService.GetirTerkNedeni(model.Id) : new MeslekTerkNedeni();
            var sonuc = 0;
            sm.ACIKLAMA = model.ACIKLAMA;
            if (model.Id == 0)
            {
                _tanimService.TerkNedenikaydet(sm);
                _uow.SaveChanges();
                sonuc = 1;
                return Json(sonuc, JsonRequestBehavior.AllowGet);
            }

            _tanimService.TerkNedenikaydet(sm);
            _uow.SaveChanges();
            sonuc = 2;
            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirIlceler()
        {
            return Json(new {IlceListesi = _tanimService.ComboBoxTumIlceler()}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DTIlceyeGoreMahalleler(int? ilceid)
        {
            var mahalleler = _tanimService.IlceyeGoreMahalleler(ilceid);
            var ds = "<table id='tblMahalleler' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>#</th><th>MAHALLE ADI</th></tr></thead><tbody>";
            foreach (var item in mahalleler)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.MAHALLE + "</td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KaydetMahalle(Mahalle model)
        {
            _tanimService.KaydetMahalle(model);
            _uow.SaveChanges();
            return Json(model.ILCEID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComboBoxIlceyeGoreMahalleler(int ilceid)
        {
            return Json(new {MahalleListesi = _tanimService.ComboBoxIlceyeGoreMahalleler(ilceid)},
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult DTMahalleyeGoreCadSokBulv(int? mahalleid)
        {
            var sokaklar = _tanimService.MahalleyeGoreCadSoklar(mahalleid);
            var ds = "<table id='tblSokaklar' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>#</th><th>BULVAR/CADDE/SOKAK ADI</th></tr></thead><tbody>";
            foreach (var item in sokaklar)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.CADSOKBULV + "</td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KaydetSokak(CadSokBulv model)
        {
            _tanimService.KaydetSokak(model);
            _uow.SaveChanges();
            return Json(model.MAHALLEID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MeslekleriGetir()
        {
            var meslekler = _tanimService.TumMeslekler();
            var ds = "<table id='tblMeslek' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>#</th><th>MESLEK KODU</th><th>MESLEK</th></tr></thead><tbody>";
            foreach (var item in meslekler)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.MESLEKKODU + "</td><td>" +
                      item.MESLEK + "</td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirMeslekler()
        {
            return Json(_tanimService.GetirMeslekler(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NaceleriGetir(int meslekid)
        {
            var naceler = _tanimService.GetirNaceler(meslekid);
            var ds = "<table id='tblNace' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>Id</th><th>NACE KODU</th><th>NACE TANIMI</th></tr></thead><tbody>";
            foreach (var item in naceler)
                ds += "<tr><td>" + item.Id + "</td><td>" + item.NACE + "</td><td>" + item.TANIMI + "</td></tr>";
            ds += "</tbody></table>";
            return Json(ds, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KaydetMeslek(Meslekler model)
        {
            var sonuc = 0;
            if (model != null)
            {
                var m = model.Id > 0 ? _tanimService.GetirMeslek(model.Id) : new Meslekler();
                m.MESLEKKODU = model.MESLEKKODU;
                m.MESLEK = model.MESLEK;
                m.MESLEKODASIID = 1;
                if (model.Id == 0)
                {
                    _tanimService.KaydetMeslek(m);
                    _uow.SaveChanges();
                    sonuc = 1;
                }
                else
                {
                    _uow.SaveChanges();
                    sonuc = 2;
                }
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KaydetNace(Nace model)
        {
            var sonuc = 0;
            if (model != null)
            {
                var m = model.Id > 0 ? _tanimService.GetirNace(model.Id) : new Nace();
                m.NACE = model.NACE;
                m.TANIMI = model.TANIMI;
                m.MESLEKID = model.MESLEKID;
                if (model.Id == 0)
                {
                    _tanimService.KaydetNace(m);
                    _uow.SaveChanges();
                    sonuc = 1;
                }
                else
                {
                    _uow.SaveChanges();
                    sonuc = 2;
                }
            }

            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }
    }
}