using System;
using System.Linq;
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
                {
                    ds += "<tr href='http://www.google.com'><td>" + item.KISAAD + "</td><td>" + item.ACIKLAMA +
                          "</td><td>Aktif</td><td>" +
                          "<div>" +
                              "<a href='#' id='" + item.Id + "' data-toggle='modal' data-target='#modalEdit' onclick='return MeslekOdasiDuzenle(" +
                              item.Id + ");' title='Düzenle' data-id='" + item.Id +
                              "'><i class='fa fa-edit'></i>" +
                          "</a>" +
                          $"<a href='#' style='margin-left:8px; color:tomato;' onclick='return odaSilQuery({item.Id});' title='Sil'><i class='fa fa-trash'></a>" +
                          "</div></td></tr>";
                }
                else
                {
                    ds += "<tr href='http://www.google.com'><td>" + item.KISAAD + "</td><td>" + item.ACIKLAMA +
      "</td><td>Pasif</td><td></td></tr>";
                }
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
        
        public JsonResult MeslekOdasiSil(int id)
        {
            var mo = _tanimService.GetirOda(id);
            if (mo != null)
            {
                _tanimService.MeslekOdasiSil(mo);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult GetirTerkNedenleri()
        {
            var model = _tanimService.GetirTerkNedenleri();
            var ds = "<table id='tblTerkNedenleri' class='display' cellspacing='0' width='100%'>" +
                     Environment.NewLine + "<thead><tr><th>#</th><th>TERK NEDENİ</th><th></th><th></th></tr></thead><tbody>";
            foreach (var item in model)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.ACIKLAMA +
                      "</td><td>" +
                      $"<a href='#' id='{item.Id}' onclick='return TerkNedeniDuzenle({item.Id});' title='Düzenle' data-id='{item.Id}'><i class='fa fa-edit'></i></a>" +
                      "</td><td>" +
                      $"<a href='#' style='margin-left:8px; color:tomato;' onclick='return terkNedeniSilQuery({item.Id});' title='Sil'><i class='fa fa-trash'></a>" +
                      $"</td></tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetirTerkNedeni(int id)
        {
            var m = _tanimService.GetirTerkNedeni(id);
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SilTerkNedeni(int id)
        {
            var m = _tanimService.GetirTerkNedeni(id);
            if(m != null)
            {
                _tanimService.TerkNedeniSil(m);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
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
                     "<thead><tr><th>#</th><th>MAHALLE ADI</th><th>Kont.</th></tr></thead><tbody>";
            foreach (var item in mahalleler)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.MAHALLE + "</td>" +
                    "<td>" +
                        "<div>" +
                            $"<a href='#' onclick='return mahalleDuzenle({ilceid},{item.Id});' title='Sil'>" +
                                "<i class='fa fa-edit'></i>" +
                            $"</a>" +
                            $"<a href='#' style='margin-left:8px; color:tomato;' onclick='return mahalleSil({ilceid},{item.Id});' title='Sil'>" +
                                "<i class='fa fa-trash'></i>" +
                            $"</a>" +
                        "</div>" +
                    "</td>" +
                    "</tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MahalleGetir(int ilceid, int mahalleID)
        {
            var mahalle = _tanimService.IlceyeGoreMahalleler(ilceid).FirstOrDefault(x => x.Id == mahalleID);
            return Json(mahalle, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MahalleDuzenle(int ilceid, int mahalleID, string yeniIsim)
        {
            var mahalle = _tanimService.IlceyeGoreMahalleler(ilceid).FirstOrDefault(x => x.Id == mahalleID);
            mahalle.MAHALLE = yeniIsim;
            var success = _tanimService.MahalleDuzenle(mahalle);
            if (success)
            {
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult MahalleSil(int ilceid, int mahalleID)
        {
            var mahalle = _tanimService.IlceyeGoreMahalleler(ilceid).FirstOrDefault(x => x.Id == mahalleID);
            var success = _tanimService.MahalleSil(mahalle);
            if (success)
            {
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
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
                     "<thead><tr><th>#</th><th>BULVAR/CADDE/SOKAK ADI</th><th></th></tr></thead><tbody>";
            foreach (var item in sokaklar)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.CADSOKBULV + "</td>" +
                        "<td>" +
                            "<div>" +
                                $"<a href='#' onclick='return cadSokBulvDuzenle({mahalleid},{item.Id});' title='Sil'>" +
                                    "<i class='fa fa-edit'></i>" +
                                $"</a>" +
                                $"<a href='#' style='margin-left:8px; color:tomato;' onclick='return cadSokBulvSil({mahalleid},{item.Id});' title='Sil'>" +
                                    "<i class='fa fa-trash'></i>" +
                                $"</a>" +
                            "</div>" +
                        "</td>" +
                    "</tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CadSokBulvGetir(int mahalleid, int cadSokBulvId)
        {
            var cadSokBulv = _tanimService.MahalleyeGoreCadSoklar(mahalleid).FirstOrDefault(x => x.Id == cadSokBulvId);
            return Json(cadSokBulv, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CadSokBulvDuzenle(int mahalleid, int cadsokbulvid, string yeniisim)
        {
            var newCadSokBulv = _tanimService.MahalleyeGoreCadSoklar(mahalleid).FirstOrDefault(x => x.Id == cadsokbulvid);
            newCadSokBulv.CADSOKBULV = yeniisim;

            var success = _tanimService.CadSokBulvDuzenle(newCadSokBulv);
            if (success)
            {
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult CadSokBulvSil(int mahalleid, int cadsokbulvid)
        {
            var cadSokBulv = _tanimService.MahalleyeGoreCadSoklar(mahalleid).FirstOrDefault(x => x.Id == cadsokbulvid);
            var success = _tanimService.CadSokBulvSil(cadSokBulv);
            if (success)
            {
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
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
                     "<thead><tr><th>#</th><th>MESLEK KODU</th><th>MESLEK</th><th></th></tr></thead><tbody>";
            foreach (var item in meslekler)
                ds += "<tr href='http://myspace.com'><td>" + item.Id + "</td><td>" + item.MESLEKKODU + "</td><td>" +
                      item.MESLEK + "</td>" +
                        "<td>" +
                            "<div>" +
                                $"<a href='#' onclick='return meslekDuzenle({item.Id});' title='Sil'>" +
                                    "<i class='fa fa-edit'></i>" +
                                $"</a>" +
                                $"<a href='#' style='margin-left:8px; color:tomato;' onclick='return meslekSil({item.Id});' title='Sil'>" +
                                    "<i class='fa fa-trash'></i>" +
                                $"</a>" +
                            "</div>" +
                        "</td>" +
                      "</tr>";
            ds += "</tbody></table>";
            return Json(new {ds}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirMeslekler()
        {
            return Json(_tanimService.GetirMeslekler(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirMeslek(int meslekid)
        {
            var meslek = _tanimService.GetirMeslek(meslekid);
            return Json(meslek, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MeslekKaydet(string MESLEK, string MESLEKKODU)
        {
            var meslek = new Meslekler() { MESLEK = MESLEK, MESLEKKODU = MESLEKKODU };
            _tanimService.KaydetMeslek(meslek);
            _uow.SaveChanges();
            return Json("basarili", JsonRequestBehavior.AllowGet);
        }

        public ActionResult MeslekDuzenle(int meslekid, string meslekkodu, string meslek)
        {
            var meslekYeni = _tanimService.GetirMeslek(meslekid);
            if(meslekYeni != null)
            {
                meslekYeni.MESLEKKODU = meslekkodu;
                meslekYeni.MESLEK = meslek;
                _tanimService.MeslekDuzenle(meslekYeni);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult MeslekSil(int meslekid)
        {
            var meslek = _tanimService.GetirMeslek(meslekid);
            if(meslek != null)
            {
                _tanimService.MeslekSil(meslek);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult NaceleriGetir(int meslekid)
        {
            var naceler = _tanimService.GetirNaceler(meslekid);
            var ds = "<table id='tblNace' class='display' cellspacing='0' width='100%'>" + Environment.NewLine +
                     "<thead><tr><th>Id</th><th>NACE KODU</th><th>NACE TANIMI</th><th></th></tr></thead><tbody>";
            foreach (var item in naceler)
                ds += "<tr><td>" + item.Id + "</td><td>" + item.NACE + "</td><td>" + item.TANIMI + "</td>" +
                        "<td>" +
                            "<div style='min-width:110px;'>" +
                                $"<a href='#' onclick='return naceDuzenle({item.Id});' title='Sil'>" +
                                    "<i class='fa fa-edit'></i>" +
                                $"</a>" +
                                $"<a href='#' style='margin-left:8px; color:tomato;' onclick='return naceSil({item.Id});' title='Sil'>" +
                                    "<i class='fa fa-trash'></i>" +
                                $"</a>" +
                            "</div>" +
                        "</td>" +
                    "</tr>";
            ds += "</tbody></table>";
            return Json(ds, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NaceGetir(int naceid)
        {
            var nace = _tanimService.GetirNace(naceid);

            return Json(nace, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NaceSil(int naceid)
        {
            var nace = _tanimService.GetirNace(naceid);

            if(nace != null)
            {
                _tanimService.NaceSil(nace);
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult NaceDuzenle(int naceid, string yenitanim, string yeninace)
        {
            var yeniNace = _tanimService.GetirNace(naceid);

            if(yeniNace != null)
            {
                yeniNace.TANIMI = yenitanim;
                yeniNace.NACE = yeninace;
                _uow.SaveChanges();
                return Json("basarili", JsonRequestBehavior.AllowGet);
            }

            return null;
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