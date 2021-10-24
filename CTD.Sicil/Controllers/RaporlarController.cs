using System;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using CTD.Core.Helpers;
using CTD.Data.UnitofWork;
using CTD.Service.Interfaces;
using CTD.Web.Framework.Controller;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CTD.Sicil.Controllers
{
    [Authorize]
    public class RaporlarController : PublicController
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly IMakbuzDokumService _makbuzDokumService;
        private readonly ISicilService _sicilService;

        public RaporlarController(IUnitofWork uow, IKullaniciService kullaniciService, ISicilService sicilService,
            IMakbuzDokumService makbuzDokumService) : base(uow)
        {
            _kullaniciService = kullaniciService;
            _sicilService = sicilService;
            _makbuzDokumService = makbuzDokumService;
        }

        public ActionResult RaporlarAnaSayfa()
        {
            return View();
        }

        public ActionResult SicilVerileri()
        {
            ViewBag.AnlikSicilVeri2 = _makbuzDokumService.SicilVerileri();
            ViewBag.TahsilatTurleri = _makbuzDokumService.GetirTahsilatTurleri();
            return View();
        }

        public ActionResult AdreseGoreUyeler()
        {
            return View();
        }

        public ActionResult AdreseGoreUyelerV2()
        {
            return View();
        }

        public ActionResult OdalaraGoreUyeler()
        {
            return View();
        }

        public ActionResult MesleklereGoreUyeler()
        {
            return View();
        }

        public ActionResult IslemYapilanUyeler()
        {
            return View();
        }

        public JsonResult Kullanicilar()
        {
            var kullanicilar = _kullaniciService.IslemYapanKullanicilar();
            var builder = "";
            foreach (var user in kullanicilar)
                builder +=
                    "<div class='tc' style='float:left;margin-right:2px;'><a href='#' onclick='return IkiTarihArasi(" +
                    user.Id + ");'><img src='/images/" + user.resim + "' class='img-circle' title='" + user.adi +
                    "' style='width:70px;'></a></div>";
            builder +=
                "<div class='col-md-1 tc' style='margin-top:5px;'><a href='#' onclick='return IkiTarihArasi(0);'><i class=\"fa fa-4x fa-group\"></i></a></div>";
            return Json(builder, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IkiTarihSicilVerileri(string ilktarih, string sontarih, int id)
        {
            var it = DateHelper.GetDateTimeCultural(ilktarih);
            var st = DateHelper.GetDateTimeCultural(sontarih);
            var htmlicerik = "";
            if (id != 0)
            {
                var liste = _makbuzDokumService.GetirKullaniciIslemSayilari(it, st, id);
                var personel = _kullaniciService.KullaniciGetir(id);
                htmlicerik = "<div><span><b>" + personel.adi + "</b> tarafından <b>" + string.Format("{0:d}", it) +
                             " - " + string.Format("{0:d}", st) +
                             "</b> tarihleri arasında yapılan işlemler ve sayıları:</span></div><br>";
                foreach (var item2 in liste)
                {
                    var islem = "";
                    if (item2.Islem == "") islem = "0";
                    foreach (var tt in _makbuzDokumService.GetirTahsilatTurleri())
                        if (tt.Id == int.Parse(item2.Islem))
                        {
                            htmlicerik += "<div class='col-md-3 col-lg-2 col-sm-4 col-xs-6'>";
                            htmlicerik += "<div class='card' onclick=\"return SicilVeriListe(" + islem + ", " +
                                          personel.Id + ")\">";
                            if (tt.Id == 2018)
                                htmlicerik += "<div class='header bg-success' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 2)
                                htmlicerik += "<div class='header bg-primary' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 3)
                                htmlicerik += "<div class='header bg-warning' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 4)
                                htmlicerik += "<div class='header bg-danger' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 5)
                                htmlicerik += "<div class='header bg-info' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 6)
                                htmlicerik += "<div class='header bg-amber' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else
                                htmlicerik += "<div class='header bg-light-blue' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            htmlicerik += "<div class='panel-body p-5 text-center'><h1>" + item2.Sayi +
                                          "</h1><small> adet işlem yapılmıştır.</small></div>";
                            htmlicerik += "</div></div>";
                            break;
                        }
                }
            }
            else
            {
                var liste = _makbuzDokumService.GetirKullaniciIslemSayilari(it, st);
                htmlicerik = "<div><span><b>Antalya Esnaf ve Sanatkarlar Sicil Müdürlüğü</b> tarafından <b>" +
                             string.Format("{0:d}", it) + " - " + string.Format("{0:d}", st) +
                             "</b> tarihleri arasında yapılan işlemler ve sayıları:</span></div><br>";
                foreach (var item2 in liste)
                {
                    var islem = "";
                    if (item2.Islem == "") islem = "0";
                    foreach (var tt in _makbuzDokumService.GetirTahsilatTurleri())
                        if (tt.Id == int.Parse(item2.Islem))
                        {
                            htmlicerik += "<div class='col-md-3 col-lg-2 col-sm-4 col-xs-6'>";
                            htmlicerik += "<div class='card' onclick=\"return SicilVeriListe(" + islem + ")\">";
                            if (tt.Id == 2018)
                                htmlicerik += "<div class='header bg-success' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 2)
                                htmlicerik += "<div class='header bg-primary' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 3)
                                htmlicerik += "<div class='header bg-warning' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 4)
                                htmlicerik += "<div class='header bg-danger' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 5)
                                htmlicerik += "<div class='header bg-info' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else if (tt.Id == 6)
                                htmlicerik += "<div class='header bg-amber' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            else
                                htmlicerik += "<div class='header bg-light-blue' style='padding:5px 0 5px 10px;'><h3>" +
                                              tt.TAHSILATTURU.ToLower() + "</h3></div>";
                            htmlicerik += "<div class='panel-body p-5 text-center'><h1>" + item2.Sayi +
                                          "</h1><small> adet işlem yapılmıştır.</small></div>";
                            htmlicerik += "</div></div>";
                            break;
                        }
                }
            }

            return Json(htmlicerik, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetirTahsilatTurleri()
        {
            return Json(new {TahsilatTurleri = _makbuzDokumService.GetirTahsilatTurleriComboBox()},
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult _YeniKayitYapilanUyeler(string ilktarih, string sontarih, int vezne)
        {
            if (ilktarih != null && sontarih != null)
            {
                var it = DateHelper.GetDateTimeCultural(ilktarih);
                var st = DateHelper.GetDateTimeCultural(sontarih);
                var data = _makbuzDokumService.YeniKayitYapilanUyeler(it, st, vezne);
                return PartialView(data);
            }

            return null;
        }

        public ActionResult ReportYeniKayitYapilanUyeler(string id, string ilktarih, string sontarih, string vezne)
        {
            var it = DateHelper.GetDateTimeCultural(ilktarih);
            var st = DateHelper.GetDateTimeCultural(sontarih);
            var liste = _makbuzDokumService.YeniKayitYapilanUyeler(it, st, int.Parse(vezne));
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
            var tableHeader = new PdfPTable(2);
            tableHeader.WidthPercentage = 100;
            tableHeader.SpacingBefore = 20f;
            tableHeader.SpacingAfter = 30f;
            float[] widths = {60f, 300f};
            tableHeader.SetWidths(widths);
            var cellHeader = new PdfPCell();
            var img = Image.GetInstance(Server.MapPath("~/Images/logo.jpg"));
            img.ScaleToFit(50, 50);
            cellHeader.Border = Rectangle.NO_BORDER;
            cellHeader.AddElement(img);
            tableHeader.AddCell(cellHeader);
            cellHeader =
                new PdfPCell(new Phrase(ilktarih + " - " + sontarih + " TARİHLERİ ARASINDA\nYENİ KAYIT YAPILAN ÜYELER",
                    proximanovaFontBold));
            cellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellHeader.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cellHeader);
            pdfDoc.Add(tableHeader);
            var table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;
            var cell1 = new PdfPCell();
            cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell1 = new PdfPCell(new Phrase("S.N", proximanovaFontBold));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.PaddingBottom = 5f;
            table.AddCell(cell1);
            cell1 = new PdfPCell(new Phrase("Scl No", proximanovaFontBold));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.PaddingBottom = 5f;
            table.AddCell(cell1);
            cell1 = new PdfPCell(new Phrase("Adı Soyadı", proximanovaFontBold));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.PaddingBottom = 5f;
            table.AddCell(cell1);
            cell1 = new PdfPCell(new Phrase("Meslek Odası", proximanovaFontBold));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.PaddingBottom = 5f;
            table.AddCell(cell1);
            cell1 = new PdfPCell(new Phrase("Mesleği", proximanovaFontBold));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.PaddingBottom = 5f;
            table.AddCell(cell1);
            var cell = new PdfPCell();
            cell.Border = 1;
            float[] dataTableWidths = {40f, 70f, 180f, 200f, 220f};
            table.SetWidths(dataTableWidths);
            var i = 0;
            foreach (var item in liste)
            {
                i = i + 1;
                cell = new PdfPCell(new Phrase(i.ToString(), proximanovaFont10));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 5f;
                cell.PaddingRight = 5f;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.SICILNO.ToString(), proximanovaFont10));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 5f;
                cell.PaddingRight = 5f;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.ADSOYAD, proximanovaFont10));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 5f;
                cell.PaddingRight = 5f;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.KISAAD, proximanovaFont10));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 5f;
                cell.PaddingRight = 5f;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.MESLEK, proximanovaFont10));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.PaddingBottom = 5f;
                cell.PaddingLeft = 5f;
                cell.PaddingRight = 5f;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);
            var dosyaAdi = DateTime.Now.Day;
            dosyaAdi = +DateTime.Now.Millisecond;
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + dosyaAdi + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            Process.Start("chrome.exe", "C:\\Downloads\\" + dosyaAdi + ".pdf");
            return View();
        }
    }
}