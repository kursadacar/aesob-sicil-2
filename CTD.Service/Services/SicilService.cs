using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using CTD.Core.Entities;
using CTD.Data.UnitofWork;
using CTD.Dto.ListedDto;
using CTD.Dto.ParamDto;
using CTD.Dto.SingleDto;
using CTD.Service.Abstracts;
using CTD.Service.Interfaces;
using CTD.Utilities.Manager;
using HtmlAgilityPack;

namespace CTD.Service.Services
{
    public class SicilService : ASicilService, ISicilService
    {
        public SicilService(IUnitofWork uow) : base(uow)
        {
        }

        public void InsertSicil(Sicil entity)
        {
            _sicilRepository.Insert(entity);
        }

        public void InsertSicilMeslek(SicilMeslek entity)
        {
            _sicilMeslekRepository.Insert(entity);
        }

        public void DeleteSicilMeslek(SicilMeslek entity)
        {
            _sicilMeslekRepository.Delete(entity);
        }

        public SicilSingleDto VerileriGetir(string dosya)
        {
            var dokuman = HtmlDuzenle(dosya);
            var sicildto = new SicilSingleDto();
            sicildto.Sicil = new Sicil();
            sicildto.SicilMeslek = new SicilMeslek();
            var sicilbasliklar = dokuman.DocumentNode.SelectNodes("//td[@class='csC35EB466']");
            var sicilveriler = dokuman.DocumentNode.SelectNodes("//td[@class='csEFFBA190']");
            foreach (var baslik in sicilbasliklar)
                if (baslik.InnerText == "T.C.&nbsp;Kimlik&nbsp;No&nbsp;&nbsp;:")
                {
                    sicildto.Sicil.TCKIMLIKNO = sicilveriler[0].InnerText;
                }
                else if (baslik.InnerText == "Ad&nbsp;Soyad&nbsp;")
                {
                    sicildto.Sicil.ADSOYAD = ReplaceText(sicilveriler[2].InnerText);
                }
                else if (baslik.InnerText == "Baba&nbsp;Adı")
                {
                    sicildto.Sicil.BABAADI = ReplaceText(sicilveriler[4].InnerText);
                }
                else if (baslik.InnerText == "Ana&nbsp;Adı")
                {
                    sicildto.Sicil.ANAADI = ReplaceText(sicilveriler[6].InnerText);
                }
                else if (baslik.InnerText == "Doğum&nbsp;Tarihi&nbsp;:")
                {
                    sicildto.Sicil.DOGTAR = Convert.ToDateTime(sicilveriler[8].InnerText);
                }
                else if (baslik.InnerText == "Doğum&nbsp;Yeri")
                {
                    sicildto.Sicil.DOGYERILCE = ReplaceText(sicilveriler[10].InnerText);
                }
                else if (baslik.InnerText == "Cinsiyet")
                {
                    var cinsiyet = sicilveriler[14].InnerText == "Erkek" ? "1" : "2";
                    sicildto.Sicil.CINSIYET = int.Parse(cinsiyet);
                }
                else if (baslik.InnerText == "Cep&nbsp;Tel")
                {
                    {
                        var sayi = sicilveriler[29].InnerText.Count();
                        if (sayi == 10) sicildto.Sicil.CEPTEL = sicilveriler[29].InnerText;
                    }
                }
                else if (baslik.InnerText == "Sicil&nbsp;No:")
                {
                    {
                        var sayi = sicilveriler[36].InnerText.Count();
                        if (sayi == 6) sicildto.Sicil.SICILNO = int.Parse(sicilveriler[36].InnerText);
                    }
                }

            var odabaslik = dokuman.DocumentNode.SelectNodes("//td[@class='csB2BC029D']");
            var oda = dokuman.DocumentNode.SelectNodes("//td[@class='cs96C63607']");
            var line = 0;
            var odam = "";
            foreach (var baslik in odabaslik)
                if (baslik.InnerText == "Oda&nbsp;Ad")
                {
                    line = baslik.Line;
                    foreach (var item in oda)
                        if (item.Line == line + 1)
                            odam = ReplaceText(item.InnerText);
                }

            var odaid = 1;
            var meslekodasi = _meslekOdasiRepository.GetAll().FirstOrDefault(c => c.ACIKLAMA == odam);
            if (meslekodasi != null)
            {
                odaid = meslekodasi.Id;
                sicildto.SicilMeslek.MESLEKODASI = odaid;
            }

            var unvanbaslik = dokuman.DocumentNode.SelectNodes("//td[@class='csB2BC029D']");
            var unvan = dokuman.DocumentNode.SelectNodes("//td[@class='cs62A1184D']");
            foreach (var baslik in unvanbaslik)
                if (baslik.InnerText == "Unvan")
                {
                    var line1 = baslik.Line;
                    var line2 = line1;
                    foreach (var item in unvan.Where(item => item.Line == line1 + 1))
                        sicildto.SicilMeslek.ISYERIUNVANI = ReplaceText(item.InnerHtml).Replace("<nobr>", "")
                            .Replace("</nobr>", "").Replace("<br>", " ");
                }

            var nacekodubaslik = dokuman.DocumentNode.SelectNodes("//td[@class='csFC81FB6D']");
            var nace = dokuman.DocumentNode.SelectNodes("//td[@class='csB54333FD']");
            var nacem = "";
            foreach (var baslik in nacekodubaslik)
                if (baslik.InnerText == "NACE&nbsp;Kodu")
                {
                    var line3 = baslik.Line;
                    var line4 = line3;
                    foreach (var item in nace.Where(item => item.Line == line4 + 1))
                        nacem = ReplaceText(item.InnerText);
                }

            sicildto.SicilMeslek.NACEKODU = int.Parse(nacem.Replace(".", "").Trim());
            var result = (from a in _mesleklerRepository.GetAll()
                join b in _naceRepository.GetAll() on a.Id equals b.MESLEKID
                where b.NACE == sicildto.SicilMeslek.NACEKODU.ToString()
                select new
                {
                    naceid = b.Id, meslekid = a.Id, meslek = a.MESLEKKODU + " - " + a.MESLEK, nacetanim = b.TANIMI
                }).SingleOrDefault();
            if (result != null)
            {
                sicildto.NaceId = result.naceid;
                sicildto.MeslekId = result.meslekid;
                sicildto.Meslek = result.meslek;
                sicildto.NaceTanimi = result.nacetanim;
            }

            var adresbaslik = dokuman.DocumentNode.SelectNodes("//td[@class='cs18AD83F5']");
            var adres = dokuman.DocumentNode.SelectNodes("//td[@class='cs3005448D']");
            var line5 = 0;
            var adresim = "";
            foreach (var baslik in adresbaslik)
                if (baslik.InnerText == "Adres")
                {
                    line5 = baslik.Line;
                    foreach (var item in adres)
                        if (item.Line == line5 + 1)
                            adresim = ReplaceText(item.InnerHtml);
                }

            adresim = adresim.Replace("<nobr>", "").Replace("</nobr>", "").Replace("<br>", " ");
            var ilcemahallesokakno = adresim.Substring(0, adresim.LastIndexOf("/ANTALYA", StringComparison.Ordinal));
            var mahallesokakno =
                ilcemahallesokakno.Substring(0, ilcemahallesokakno.LastIndexOf(" ", StringComparison.Ordinal));
            var mahallesokak = mahallesokakno.Substring(0, mahallesokakno.LastIndexOf("NO", StringComparison.Ordinal));
            var mahalle = adresim.Substring(0, adresim.IndexOf(" MAHALLESİ", StringComparison.Ordinal));
            var ilcemahallesokaknosayi = ilcemahallesokakno.Length;
            var mahallesokaknosayi = mahallesokakno.Length;
            var mahallesokaksayi = mahallesokak.Length;
            var mahallesayi = mahalle.Length + 10;
            var ilce = adresim.Substring(mahallesokaknosayi, ilcemahallesokaknosayi - mahallesokaknosayi).Trim();
            var sokak = mahallesokak.Substring(mahallesayi, mahallesokaksayi - mahallesayi).Trim();
            sokak = sokak.Substring(0, sokak.LastIndexOf(" ", StringComparison.Ordinal));
            var no = adresim.Substring(mahallesokaksayi, mahallesokaknosayi - mahallesokaksayi).Trim();
            var tblilce = _ilceRepository.GetAll().FirstOrDefault(i => i.ILCE == ilce);
            var ilceid = 1;
            var mahalleid = 1;
            var caddesokakid = 26789;
            if (tblilce != null)
            {
                ilceid = tblilce.Id;
                var tblmahalle = _mahalleRepository.GetAll()
                    .FirstOrDefault(s => s.ILCEID == ilceid && s.MAHALLE.Contains(mahalle));
                if (tblmahalle != null)
                {
                    mahalleid = tblmahalle.Id;
                    var tblcaddesokak = _cadSokBulvRepository.GetAll().FirstOrDefault(t =>
                        t.ILCEID == ilceid && t.MAHALLEID == mahalleid && t.CADSOKBULV.Contains(sokak));
                    if (tblcaddesokak != null) caddesokakid = tblcaddesokak.Id;
                }
            }

            sicildto.SicilMeslek.ISADRESILCE = ilceid;
            sicildto.SicilMeslek.ISADRESMAHALLE = mahalleid;
            sicildto.SicilMeslek.ISADRESCADSOKBULV = caddesokakid;
            sicildto.Adres = no;
            return sicildto;
        }

        public Sicil VerileriGetirKisisel(string dosya)
        {
            var sicil = new Sicil();
            var dokuman = HtmlDuzenle(dosya);
            var basliklar = dokuman.DocumentNode.SelectNodes("//td[@class='csC35EB466']");
            var veriler = dokuman.DocumentNode.SelectNodes("//td[@class='csEFFBA190']");
            foreach (var baslik in basliklar)
                if (baslik.InnerText == "T.C.&nbsp;Kimlik&nbsp;No&nbsp;&nbsp;:")
                {
                    sicil.TCKIMLIKNO = veriler[0].InnerText;
                }
                else if (baslik.InnerText == "Ad&nbsp;Soyad&nbsp;")
                {
                    sicil.ADSOYAD = ReplaceText(veriler[2].InnerText);
                }
                else if (baslik.InnerText == "Baba&nbsp;Adı")
                {
                    sicil.BABAADI = ReplaceText(veriler[4].InnerText);
                }
                else if (baslik.InnerText == "Ana&nbsp;Adı")
                {
                    sicil.ANAADI = ReplaceText(veriler[6].InnerText);
                }
                else if (baslik.InnerText == "Doğum&nbsp;Tarihi&nbsp;:")
                {
                    sicil.DOGTAR = Convert.ToDateTime(veriler[8].InnerText);
                }
                else if (baslik.InnerText == "Doğum&nbsp;Yeri")
                {
                    sicil.DOGYERILCE = ReplaceText(veriler[10].InnerText);
                }
                else if (baslik.InnerText == "Cinsiyet")
                {
                    var cinsiyet = veriler[14].InnerText == "Erkek" ? "1" : "2";
                    sicil.CINSIYET = int.Parse(cinsiyet);
                }
                else if (baslik.InnerText == "Cep&nbsp;Tel")
                {
                    {
                        var sayi = veriler[29].InnerText.Count();
                        if (sayi == 10) sicil.CEPTEL = veriler[29].InnerText;
                    }
                }
                else if (baslik.InnerText == "Sicil&nbsp;No:")
                {
                    {
                        var sayi = veriler[36].InnerText.Count();
                        if (sayi == 6) sicil.SICILNO = int.Parse(veriler[36].InnerText);
                    }
                }

            return sicil;
        }

        public int VerileriGetirOda(string dosya)
        {
            var dokuman = HtmlDuzenle(dosya);
            var odabaslik = dokuman.DocumentNode.SelectNodes("//td[@class='csB2BC029D']");
            var oda = dokuman.DocumentNode.SelectNodes("//td[@class='cs96C63607']");
            var odam = "";
            foreach (var baslik in odabaslik)
                if (baslik.InnerText == "Oda&nbsp;Ad")
                {
                    var line = baslik.Line;
                    foreach (var item in oda)
                        if (item.Line == line + 1)
                            odam = ReplaceText(item.InnerText);
                }

            var odaid = 1;
            var meslekodasi = _meslekOdasiRepository.GetAll().FirstOrDefault(c => c.ACIKLAMA == odam);
            if (meslekodasi != null) odaid = meslekodasi.Id;
            return odaid;
        }

        public string CheckSicilNo(int sicilno)
        {
            var result = _sicilRepository.GetAll().Count(s => s.SICILNO == sicilno);
            return result > 0 ? "sicilvar" : "sicilyok";
        }

        public string CheckTcKimlik(string tcno)
        {
            var result = _sicilRepository.GetAll().Count(s => s.TCKIMLIKNO == tcno);
            return result > 0 ? "tcvar" : "tcyok";
        }

        public List<ComboBoxIdTextDto> GetirSiniflar()
        {
            return _sinifRepository.GetAll().OrderBy(p => p.Id).ToList()
                .Select(p => new ComboBoxIdTextDto {text = p.SINIF, id = p.Id.ToString()}).ToList();
        }

        public List<ComboBoxIdTextDto> GetirOdalar()
        {
            return _meslekOdasiRepository.GetAll().OrderBy(p => p.KISAAD).ToList()
                .Select(p => new ComboBoxIdTextDto {text = p.KISAAD, id = p.Id.ToString()}).ToList();
        }

        public List<ComboBoxIdTextDto> GetirMeslekler()
        {
            return _mesleklerRepository.GetAll().OrderBy(p => p.MESLEK).ToList()
                .Select(p => new ComboBoxIdTextDto {text = p.MESLEK, id = p.Id.ToString()}).ToList();
        }

        public List<ComboBoxIdTextDto> GetirIlceler()
        {
            return _ilceRepository.GetAll().OrderBy(p => p.ILCE).ToList()
                .Select(p => new ComboBoxIdTextDto {text = p.ILCE, id = p.Id.ToString()}).ToList();
        }

        public Ilce GetirIlce(int? id)
        {
            return _ilceRepository.GetAll().SingleOrDefault(s => s.Id == id);
        }

        public Mahalle GetirMahalle(int? id)
        {
            return _mahalleRepository.GetAll().SingleOrDefault(t => t.Id == id);
        }

        public CadSokBulv GetirCadSokBulv(int? id)
        {
            return _cadSokBulvRepository.GetAll().SingleOrDefault(t => t.Id == id);
        }

        public Nace GetirMeslekKodu(string nace)
        {
            return _naceRepository.GetAll().SingleOrDefault(t => t.NACE == nace);
        }

        public string GetirOda(int oda)
        {
            return _meslekOdasiRepository.GetAll().FirstOrDefault(o => o.Id == oda).ACIKLAMA;
        }

        public string GetirMeslek(int? id)
        {
            return _mesleklerRepository.GetAll().FirstOrDefault(o => o.Id == id).MESLEK;
        }

        public List<ComboBoxIdTextDto> GetirIlceyeGoreMahalle(int? id)
        {
            var result = (from a in _mahalleRepository.GetAll() where a.ILCEID == id orderby a.MAHALLE select a)
                .ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.MAHALLE}).ToList();
            return result;
        }

        public List<ComboBoxIdTextDto> GetirMahalleyeGoreCadSokBulv(int? id)
        {
            var result =
                (from a in _cadSokBulvRepository.GetAll() where a.MAHALLEID == id orderby a.CADSOKBULV select a)
                .ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.CADSOKBULV}).ToList();
            return result;
        }

        public List<SicilAramaDto> GetirUyeler(string arama)
        {
            if (IsNumeric(arama))
            {
                var text = arama.Trim();
                var sayac = 0;
                foreach (var karakter in text) sayac++;
                if (sayac > 0 && sayac < 7)
                {
                    var search = int.Parse(arama);
                    var kayitsayisi = _sicilRepository.GetAll().Count(t => t.SICILNO == search);
                    if (kayitsayisi == 1)
                    {
                        var m = _sicilRepository.GetAll().SingleOrDefault(t => t.SICILNO == search);
                        if (m == null) return null;
                        var sadto = new SicilAramaDto();
                        var liste = new List<SicilAramaDto>();
                        sadto.AdSoyad = m.ADSOYAD;
                        sadto.BabaAdi = m.BABAADI;
                        sadto.AnneAdi = m.ANAADI;
                        sadto.DogumTarihi = m.DOGTAR.ToString();
                        sadto.SicilNo = int.Parse(m.SICILNO.ToString());
                        sadto.Durum = "tekkayit";
                        liste.Add(sadto);
                        return liste;
                    }

                    if (kayitsayisi > 1)
                    {
                        var model = _sicilRepository.GetAll().Where(t => t.SICILNO == search).ToList();
                        var liste = new List<SicilAramaDto>();
                        foreach (var item in model)
                        {
                            var sadto = new SicilAramaDto();
                            sadto.AdSoyad = item.ADSOYAD;
                            sadto.BabaAdi = item.BABAADI;
                            sadto.AnneAdi = item.ANAADI;
                            sadto.TcKimlikNo = item.TCKIMLIKNO;
                            var it = Convert.ToDateTime(item.DOGTAR.ToString());
                            sadto.DogumTarihi = string.Format("{0:d}", it);
                            sadto.SicilNo = int.Parse(item.SICILNO.ToString());
                            sadto.Durum = "tckimliknoyagoreliste";
                            liste.Add(sadto);
                        }

                        return liste;
                    }
                }
                else if (sayac > 6 && sayac < 12)
                {
                    var tckimlik = Convert.ToString(arama);
                    var kayitsayisi = _sicilRepository.GetAll().Count(t => t.TCKIMLIKNO == tckimlik);
                    if (kayitsayisi == 1)
                    {
                        var m = _sicilRepository.GetAll().SingleOrDefault(t => t.TCKIMLIKNO == tckimlik);
                        if (m == null) return null;
                        var sadto = new SicilAramaDto();
                        var liste = new List<SicilAramaDto>();
                        sadto.AdSoyad = m.ADSOYAD;
                        sadto.BabaAdi = m.BABAADI;
                        sadto.AnneAdi = m.ANAADI;
                        sadto.DogumTarihi = m.DOGTAR.ToString();
                        sadto.SicilNo = int.Parse(m.SICILNO.ToString());
                        sadto.Durum = "tekkayit";
                        liste.Add(sadto);
                        return liste;
                    }

                    if (kayitsayisi > 1)
                    {
                        var model = _sicilRepository.GetAll().Where(t => t.TCKIMLIKNO == tckimlik).ToList();
                        var liste = new List<SicilAramaDto>();
                        foreach (var item in model)
                        {
                            var sadto = new SicilAramaDto();
                            sadto.AdSoyad = item.ADSOYAD;
                            sadto.BabaAdi = item.BABAADI;
                            sadto.AnneAdi = item.ANAADI;
                            sadto.TcKimlikNo = item.TCKIMLIKNO;
                            var it = Convert.ToDateTime(item.DOGTAR.ToString());
                            sadto.DogumTarihi = string.Format("{0:d}", it);
                            sadto.SicilNo = int.Parse(item.SICILNO.ToString());
                            sadto.Durum = "tckimliknoyagoreliste";
                            liste.Add(sadto);
                        }

                        return liste;
                    }
                }
            }
            else
            {
                var adsoyad = Convert.ToString(arama);
                var kayitsayisi = _sicilRepository.GetAll().Count(t => t.ADSOYAD.Contains(arama));
                if (kayitsayisi == 1)
                {
                    var m = _sicilRepository.GetAll().SingleOrDefault(t => t.ADSOYAD.Contains(adsoyad));
                    if (m == null) return null;
                    var sadto = new SicilAramaDto();
                    var liste = new List<SicilAramaDto>();
                    sadto.AdSoyad = m.ADSOYAD;
                    sadto.BabaAdi = m.BABAADI;
                    sadto.AnneAdi = m.ANAADI;
                    sadto.DogumTarihi = m.DOGTAR.ToString();
                    sadto.SicilNo = int.Parse(m.SICILNO.ToString());
                    sadto.Durum = "tekkayit";
                    liste.Add(sadto);
                    return liste;
                }

                if (kayitsayisi > 1)
                {
                    var model = _sicilRepository.GetAll().Where(t => t.ADSOYAD.Contains(adsoyad)).ToList();
                    var liste = new List<SicilAramaDto>();
                    foreach (var item in model)
                    {
                        var sadto = new SicilAramaDto();
                        sadto.AdSoyad = item.ADSOYAD;
                        sadto.BabaAdi = item.BABAADI;
                        sadto.AnneAdi = item.ANAADI;
                        sadto.TcKimlikNo = item.TCKIMLIKNO;
                        var it = Convert.ToDateTime(item.DOGTAR.ToString());
                        sadto.DogumTarihi = string.Format("{0:d}", it);
                        sadto.SicilNo = int.Parse(item.SICILNO.ToString());
                        sadto.Durum = "tckimliknoyagoreliste";
                        liste.Add(sadto);
                    }

                    return liste;
                }
            }

            return null;
        }

        public SicilDetailDto SicilDetail(int sicilno)
        {
            var sicil = _sicilRepository.GetAll().FirstOrDefault(s => s.SICILNO == sicilno);
            var sicilmeslek = _sicilMeslekRepository.GetAll().Where(sm => sm.SICILID == sicil.Id).ToList();
            var sicilid = int.Parse(sicil.Id.ToString());
            var yapilanislemler = (from c in _tahsilatIslemleriRepository.GetAll()
                where c.SICILID == sicilid
                select new TahsilatIslemleriDto
                    {User = c.USER, IslemTuru = c.ISLEMTURU, Meslek = c.MESLEK, IslemTarihi = c.ISLEMTARIHI}).ToList();
            var sicildetail = new SicilDetailDto
            {
                Sicil = sicil, SicilMeslekler = sicilmeslek, GecmisIslemler = yapilanislemler,
                Durum = DurumuGetir(sicilmeslek)
            };
            return sicildetail;
        }

        public void SicilGuncelle(Sicil model)
        {
            _sicilRepository.Update(model);
        }

        public Sicil SicilGetir(int? id)
        {
            return _sicilRepository.GetAll().FirstOrDefault(s => s.Id == id);
        }

        public SicilDto SicilDtoGetir(int? id)
        {
            var sicildto = (from c in _sicilRepository.GetAll()
                where c.Id == id
                select new SicilDto
                {
                    Id = c.Id, Aciklama = c.ACIKLAMA, AdSoyad = c.ADSOYAD, AnaAdi = c.ANAADI, BabaAdi = c.BABAADI,
                    CepTel = c.CEPTEL, Cinsiyet = c.CINSIYET, DegEden = c.DEGEDEN, DegTar = c.DEGTAR, DogTrh = c.DOGTAR,
                    DogYerIl = c.DOGYERIL, DogYerIlce = c.DOGYERILCE, KayitEden = c.KAYITEDEN, KayitTar = c.KAYITTAR,
                    SicilNo = c.SICILNO, TCKimlikNo = c.TCKIMLIKNO
                }).SingleOrDefault();
            return sicildto;
        }

        public void KaydetSicilMeslekDegisiklik_log(SicilMeslekDegisiklik_Log model)
        {
            _sicilMeslekDegisiklik_LogRepository.Insert(model);
        }

        public List<SicilMeslekDto> SicilMeslekListesi(int? id)
        {
            if (id > 0)
            {
                DateTime? bosTarih = null;
                var list = (from sm in _sicilMeslekRepository.GetAll()
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.SICILID == id
                    select new SicilMeslekDto
                    {
                        ID = sm.Id, SICILID = sm.SICILID, MESLEKODASIKISAAD = mo.KISAAD, MESLEKADI = m.MESLEK,
                        ISADRES2 = sm.ISADRES2,
                        VIZESURESIBITTAR = sm.VIZESURESIBITTAR == bosTarih ? null : sm.VIZESURESIBITTAR,
                        MESLEKTERKTAR = sm.MESLEKTERKTAR == bosTarih ? null : sm.MESLEKTERKTAR,
                        KAYITTAR = sm.KAYITTAR == bosTarih ? null : sm.KAYITTAR
                    }).ToList();
                return list;
            }

            return null;
        }

        public SicilDetailMeslekDto SicilDetailMeslek(int? id)
        {
            if (id > 0)
            {
                var md = (from sm in _sicilMeslekRepository.GetAll()
                    join si in _sicilRepository.GetAll() on sm.SICILID equals si.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    join s in _sinifRepository.GetAll() on sm.SINIF equals s.Id
                    join n in _naceRepository.GetAll() on sm.NACEKODU equals n.Id
                    join t in _meslekTerkNedeniRepository.GetAll() on sm.MESLEKTERKNEDENI equals t.Id
                    join k in _kullaniciRepository.GetAll() on sm.KAYITEDEN equals k.Id
                    where sm.Id == id
                    select new SicilDetailMeslekDto
                    {
                        Id = sm.Id, SicilId = sm.SICILID, IsyeriUnvani = sm.ISYERIUNVANI, EskiMeslek = sm.ESKI_MESLEK,
                        Merkez = sm.MERKEZ, OdaKisaAd = mo.KISAAD, SinifString = s.SINIF, MeslekAdi = m.MESLEK,
                        MeslekKodu = m.MESLEKKODU, IsAdres2 = sm.ISADRES2, NaceAltiliKod = n.NACE, NaceTanim = n.TANIMI,
                        MeslekTerkNedeniString = t.ACIKLAMA, VizeSuresiBitTar = sm.VIZESURESIBITTAR,
                        MeslekTerkTar = sm.MESLEKTERKTAR, KayitTar = sm.KAYITTAR, SonVizeIsTar = sm.SONVIZEISTAR,
                        SonDegisTar = sm.SONDEGISTAR, Aciklama = sm.ACIKLAMA, KayitEden = k.adi
                    }).SingleOrDefault();
                return md;
            }

            return null;
        }

        public SicilMeslekDuzenleDto SicilMeslekDuzenle(int? id)
        {
            var sicilMeslek = new SicilMeslekDuzenleDto();
            var sicilDetay = _sicilMeslekRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (sicilDetay != null)
            {
                sicilMeslek.Ilces = GetirIlceler(sicilDetay.ISADRESILCE);
                sicilMeslek.Mahalles = GetirIlceyeGoreMahalle(sicilDetay.ISADRESILCE);
                sicilMeslek.CadSokBulvs = GetirMahalleyeGoreCadSokBulv(sicilDetay.ISADRESMAHALLE);
                sicilMeslek.MeslekOdasis = GetirOdalar();
                sicilMeslek.Sinifs = GetirSiniflar();
                sicilMeslek.MeslekTerkNedenis = GetirTerkNedeni();
                sicilMeslek.IsAdresIlce = sicilDetay.ISADRESILCE;
                sicilMeslek.IsAdresMahalle = sicilDetay.ISADRESMAHALLE;
                sicilMeslek.IsAdresCadSokBulv = sicilDetay.ISADRESCADSOKBULV;
                sicilMeslek.MeslekTerkNedeni = sicilDetay.MESLEKTERKNEDENI;
                sicilMeslek.MeslekOdasi = sicilDetay.MESLEKODASI;
                sicilMeslek.Sinif = sicilDetay.SINIF;
                sicilMeslek.IsAdres = sicilDetay.ISADRES;
                sicilMeslek.IsAdres2 = sicilDetay.ISADRES2;
                sicilMeslek.NaceId = sicilDetay.NACEKODU;
                sicilMeslek.NaceKodu = Nace(sicilDetay.NACEKODU).NACE;
                sicilMeslek.NaceTanimi = Nace(sicilDetay.NACEKODU).TANIMI;
                sicilMeslek.MeslekKodu = Meslek(Nace(sicilDetay.NACEKODU).MESLEKID).MESLEKKODU;
                sicilMeslek.MeslekId = Meslek(Nace(sicilDetay.NACEKODU).MESLEKID).Id;
                sicilMeslek.MeslekTanimi = Meslek(Nace(sicilDetay.NACEKODU).MESLEKID).MESLEK;
                sicilMeslek.IsyeriUnvani = sicilDetay.ISYERIUNVANI;
                sicilMeslek.KayitTar = sicilDetay.KAYITTAR;
                sicilMeslek.SonVizeIsTar = sicilDetay.SONVIZEISTAR;
                sicilMeslek.SonDegisTar = sicilDetay.SONDEGISTAR;
                sicilMeslek.MeslekTerkTar = sicilDetay.MESLEKTERKTAR;
                sicilMeslek.VizeSuresiBitTar = sicilDetay.VIZESURESIBITTAR;
                sicilMeslek.Aciklama = sicilDetay.ACIKLAMA;
                sicilMeslek.Id = id;
                sicilMeslek.SicilId = sicilDetay.SICILID;
            }

            return sicilMeslek;
        }

        public void UpdateSicilMeslekRecord(SicilMeslek model)
        {
            _sicilMeslekRepository.Update(model);
        }

        public SicilMeslek SicilMeslekGetir(int? id)
        {
            return _sicilMeslekRepository.GetAll().FirstOrDefault(sm => sm.Id == id);
        }

        public NaceMeslekParamDto NaceBilgileriGetir(string nacekodu)
        {
            var veri = (from k in _mesleklerRepository.GetAll()
                    join p in _naceRepository.GetAll() on k.Id equals p.MESLEKID
                    where p.NACE == nacekodu
                    select new NaceMeslekParamDto
                    {
                        Meslek = k.MESLEK, MeslekKodu = k.MESLEKKODU, Tanimi = p.TANIMI, MeslekId = p.MESLEKID,
                        Id = p.Id
                    })
                .SingleOrDefault();
            return veri;
        }

        public Nace NaceBilgileriGetir2(int naceid)
        {
            return _naceRepository.GetAll().FirstOrDefault(nace => nace.Id == naceid);
        }

        public SicilMeslekDuzenleDto MerkezBilgileriniGetir(int? id)
        {
            var model = _sicilMeslekRepository.GetAll().FirstOrDefault(sm => sm.Id == id);
            if (model != null)
            {
                var meslekOdasi = _meslekOdasiRepository.GetAll().FirstOrDefault(mo => mo.Id == model.MESLEKODASI);
                var naceKodu = _naceRepository.GetAll().FirstOrDefault(n => n.Id == model.NACEKODU);
                var meslek = _mesleklerRepository.GetAll().FirstOrDefault(m => m.Id == model.MESLEK);
                var kayittarihi = DateTime.Now;
                if (meslekOdasi != null)
                {
                    var sicilmeslek = new SicilMeslekDuzenleDto
                    {
                        Id = model.Id, SicilId = model.SICILID, NaceId = model.NACEKODU,
                        MeslekOdasi = model.MESLEKODASI, Sinif = model.SINIF, MeslekId = model.MESLEK,
                        MeslekOdasiString = meslekOdasi.KISAAD, NaceKoduString = naceKodu.NACE,
                        NaceTanimi = naceKodu.TANIMI, MeslekKodu = meslek.MESLEKKODU, MeslekTanimi = meslek.MESLEK,
                        KayitTar = kayittarihi, VizeSuresiBitTar = kayittarihi.AddYears(5), Ilces = GetirIlceler()
                    };
                    return sicilmeslek;
                }
            }

            return null;
        }

        public YeniMeslekDto YeniMeslekCombolariGetir()
        {
            var ym = new YeniMeslekDto {Ilces = GetirIlceler(), MeslekOdasis = GetirOdalar(), Sinifs = GetirSiniflar()};
            return ym;
        }

        public SicilSgkYazismaDto SicilSgkBilgi(int id)
        {
            var sgk = new SicilSgkYazismaDto();
            var kayit = new DateTime();
            var sicil = _sicilRepository.GetAll().FirstOrDefault(c => c.SICILNO == id);
            var durum = _sicilMeslekRepository.GetAll().Count(a => a.SICILID == id && a.MESLEKTERKTAR == null);
            if (durum > 0)
            {
                var lst = _sicilMeslekRepository.GetAll().Where(d => d.SICILID == sicil.Id && d.MESLEKTERKTAR == null)
                    .OrderBy(t => t.KAYITTAR).FirstOrDefault();
                if (lst?.KAYITTAR != null) kayit = (DateTime) lst.KAYITTAR;
                if (sicil != null)
                {
                    sgk.AdSoyad = sicil.ADSOYAD;
                    sgk.BabaAdi = sicil.BABAADI;
                    if (sicil.DOGTAR != null) sgk.DogumTarihi = (DateTime) sicil.DOGTAR;
                    if (kayit != null) sgk.Kayittarihi = kayit;
                    sgk.TcKimlikNo = " (TCKN: " + sicil.TCKIMLIKNO + ")";
                    sgk.Durum = durum;
                    sgk.SicilNo = id;
                }
            }
            else if (durum == 0)
            {
                var lst = _sicilMeslekRepository.GetAll().Where(d => d.SICILID == sicil.Id).OrderBy(t => t.KAYITTAR)
                    .FirstOrDefault();
                if (lst?.KAYITTAR != null) kayit = (DateTime) lst.KAYITTAR;
                if (sicil != null)
                {
                    sgk.AdSoyad = sicil.ADSOYAD;
                    sgk.BabaAdi = sicil.BABAADI;
                    if (sicil.DOGTAR != null) sgk.DogumTarihi = (DateTime) sicil.DOGTAR;
                    if (kayit != null) sgk.Kayittarihi = kayit;
                    sgk.TcKimlikNo = " (TCKN: " + sicil.TCKIMLIKNO + ")";
                    sgk.Durum = durum;
                    sgk.SicilNo = id;
                }
            }

            return sgk;
        }

        public List<YazismaReportList> YazismaReportLists(int sicilid)
        {
            var sicilmeslek = _sicilMeslekRepository.GetAll().Where(sm => sm.SICILID == sicilid).ToList();
            var liste = new List<YazismaReportList>();
            foreach (var item in sicilmeslek)
            {
                var yrl = new YazismaReportList();
                yrl.KAYITTAR = item.KAYITTAR;
                yrl.MESLEKTERKTAR = item.MESLEKTERKTAR;
                var mesleki = _mesleklerRepository.GetAll().FirstOrDefault(m => m.Id == item.MESLEK);
                if (item.MESLEK == 0 && item.ESKI_MESLEK != null)
                    yrl.meslek = item.ESKI_MESLEK;
                else
                    yrl.meslek = mesleki.MESLEK;
                liste.Add(yrl);
            }

            return liste;
        }

        public string SicilNoKontrol(int? sicilno)
        {
            var uye = _sicilRepository.GetAll().FirstOrDefault(u => u.SICILNO == sicilno).ADSOYAD;
            return uye;
        }

        public List<UnvanAramaDto> UnvanArama(string kriter)
        {
            var unvan = (from sm in _sicilMeslekRepository.GetAll()
                join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                join i in _ilceRepository.GetAll() on sm.ISADRESILCE equals i.Id
                join o in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals o.Id
                where sm.ISYERIUNVANI.Contains(kriter) && sm.MESLEKTERKTAR == null
                select new UnvanAramaDto
                {
                    IsyeriUnvani = sm.ISYERIUNVANI, SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, Meslek = m.MESLEK,
                    Odasi = o.KISAAD, Ilce = i.ILCE
                }).ToList();
            return unvan;
        }

        public List<SonSicilMeslekListesi> SonSicilMeslekListe(int sicilid)
        {
            var t = (from sm in _sicilMeslekRepository.GetAll()
                join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                join n in _naceRepository.GetAll() on sm.NACEKODU equals n.Id
                join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                join mh in _mahalleRepository.GetAll() on sm.ISADRESMAHALLE equals mh.Id
                join cd in _cadSokBulvRepository.GetAll() on sm.ISADRESCADSOKBULV equals cd.Id
                join i in _ilceRepository.GetAll() on sm.ISADRESILCE equals i.Id
                join iy in _kullaniciRepository.GetAll() on sm.DEGEDEN equals iy.Id
                where sm.MESLEKTERKTAR == null && sm.SICILID == sicilid
                select new SonSicilMeslekListesi
                {
                    id = sm.Id, isyeriunvani = sm.ISYERIUNVANI, sinifi = sm.SINIF, kayittarihi = sm.KAYITTAR,
                    vizetarihi = sm.VIZESURESIBITTAR, adres = sm.ISADRES, islemtarihi = sm.DEGTAR, odasi = mo.KISAAD,
                    nacekodu = n.NACE, meslekkodu = m.MESLEKKODU, meslek = m.MESLEK, mahalle = mh.MAHALLE,
                    cadde = cd.CADSOKBULV, ilce = i.ILCE, islemiyapan = iy.adi
                }).ToList();
            return t;
        }

        public List<SonSicilMeslekDegisiklik_LogListesi> SonSicilMeslekDegisiklikListe(int sicilid)
        {
            var t = (from sm in _sicilMeslekDegisiklik_LogRepository.GetAll()
                join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                join n in _naceRepository.GetAll() on sm.NACEKODU equals n.Id
                join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                join iy in _kullaniciRepository.GetAll() on sm.ISLEMIYAPAN equals iy.Id
                where sm.SICILID == sicilid
                select new SonSicilMeslekDegisiklik_LogListesi
                {
                    Id = sm.Id, SicilMeslekId = sm.SICILMESLEKID, IsyeriUnvani = sm.ISYERIUNVANI, Sinifi = sm.SINIF,
                    KayitTarihi = sm.KAYITTAR, VizeTarihi = sm.VIZESURESIBITTAR, Adres = sm.ISADRES,
                    IslemTarihi = sm.ISLEMTARIHI, Odasi = mo.KISAAD, NaceKodu = n.NACE, MeslekKodu = m.MESLEKKODU,
                    Meslek = m.MESLEK, Mahalle = "", Cadde = "", Ilce = "", IslemiYapan = iy.adi
                }).ToList();
            return t;
        }

        public void SilSicilMeslekDegisiklik_Log(List<SicilMeslekDegisiklik_Log> model)
        {
            foreach (var item in model) _sicilMeslekDegisiklik_LogRepository.Delete(item);
        }

        public List<SicilMeslekDegisiklik_Log> GetirSicilMeslekDegisiklik_Log(int sicilid)
        {
            return _sicilMeslekDegisiklik_LogRepository.GetAll().Where(c => c.SICILID == sicilid).ToList();
        }

        public int GetirSicilMeslekDegisiklik_LogSayisi(int sicilid)
        {
            var dt = Convert.ToDateTime(string.Format("{0:dd/MM/yyyy}", DateTime.Now));
            var sayi = _sicilMeslekDegisiklik_LogRepository.GetAll()
                .Where(c => c.SICILID == sicilid && c.ISLEMTARIHI == dt).Count();
            return sayi;
        }

        public List<SicilMeslek> SicilMeslekMevcutDurum(int sicilid)
        {
            var liste = _sicilMeslekRepository.GetAll().Where(c => c.SICILID == sicilid && c.MESLEKTERKTAR == null)
                .ToList();
            return liste;
        }

        public List<SicilAramaDto> DetayliArama(string adsoyad, string babaadi, string meslekodasi, string meslek)
        {
            if (meslekodasi == "" && meslek == "")
            {
                var liste = (from s in _sicilRepository.GetAll()
                    where s.ADSOYAD.Contains(adsoyad) && s.BABAADI.Contains(babaadi)
                    select new SicilAramaDto
                    {
                        AdSoyad = s.ADSOYAD, TcKimlikNo = s.TCKIMLIKNO, BabaAdi = s.BABAADI, AnneAdi = s.ANAADI,
                        DogumTarihi = s.DOGTAR.ToString(), SicilNo = s.SICILNO
                    }).ToList();
                return liste;
            }

            if (meslekodasi == "" && meslek != "")
            {
                var meslekid = int.Parse(meslek);
                var liste = (from s in _sicilRepository.GetAll()
                    join sm in _sicilMeslekRepository.GetAll() on s.Id equals sm.SICILID
                    where s.ADSOYAD.Contains(adsoyad) && s.BABAADI.Contains(babaadi) && sm.MESLEK == meslekid
                    select new SicilAramaDto
                    {
                        AdSoyad = s.ADSOYAD, TcKimlikNo = s.TCKIMLIKNO, BabaAdi = s.BABAADI, AnneAdi = s.ANAADI,
                        DogumTarihi = s.DOGTAR.ToString(), SicilNo = s.SICILNO
                    }).Distinct().ToList();
                return liste;
            }

            if (meslekodasi != "" && meslek == "")
            {
                var meslekodasiid = int.Parse(meslekodasi);
                var liste = (from s in _sicilRepository.GetAll()
                    join sm in _sicilMeslekRepository.GetAll() on s.Id equals sm.SICILID
                    where s.ADSOYAD.Contains(adsoyad) && s.BABAADI.Contains(babaadi) && sm.MESLEKODASI == meslekodasiid
                    select new SicilAramaDto
                    {
                        AdSoyad = s.ADSOYAD, TcKimlikNo = s.TCKIMLIKNO, BabaAdi = s.BABAADI, AnneAdi = s.ANAADI,
                        DogumTarihi = s.DOGTAR.ToString(), SicilNo = s.SICILNO
                    }).Distinct().ToList();
                return liste;
            }

            if (meslekodasi != "" && meslek != "")
            {
                var meslekodasiid = int.Parse(meslekodasi);
                var meslekid = int.Parse(meslek);
                var liste = (from s in _sicilRepository.GetAll()
                    join sm in _sicilMeslekRepository.GetAll() on s.Id equals sm.SICILID
                    where s.ADSOYAD.Contains(adsoyad) && s.BABAADI.Contains(babaadi) &&
                          sm.MESLEKODASI == meslekodasiid && sm.MESLEK == meslekid
                    select new SicilAramaDto
                    {
                        AdSoyad = s.ADSOYAD, TcKimlikNo = s.TCKIMLIKNO, BabaAdi = s.BABAADI, AnneAdi = s.ANAADI,
                        DogumTarihi = s.DOGTAR.ToString(), SicilNo = s.SICILNO
                    }).Distinct().ToList();
                return liste;
            }

            return null;
        }

        public void InsertTahsilatIslemleri(TahsilatIslemleriDto model)
        {
            var log = new TahsilatIslemleri
            {
                ISLEMTARIHI = model.IslemTarihi, ISLEMTURU = model.IslemTuru, ISLEMTURUID = model.IslemTuruId,
                MESLEK = model.Meslek, SICILID = int.Parse(model.SicilId.ToString()),
                SICILMESLEKID = model.SicilMeslekId, USER = model.User, USERID = model.UserId
            };
            _tahsilatIslemleriRepository.Insert(log);
        }

        public SicilMeslekDegisiklik_Log GetirSicilMeslekDegisiklik_Log(int sicilid, int sicilmeslekid)
        {
            var dt = Convert.ToDateTime(string.Format("{0:dd/MM/yyyy}", DateTime.Now));
            return _sicilMeslekDegisiklik_LogRepository.GetAll().OrderByDescending(s => s.Id).FirstOrDefault(c =>
                c.SICILID == sicilid && c.SICILMESLEKID == sicilmeslekid && c.ISLEMTARIHI == dt);
        }

        public List<string> VizesiGelenler()
        {
            var veriler = new List<string>();
            var cepler = "";
            var sayac = 0;
            var gununtarihi = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            var d2 = gununtarihi.AddDays(-6);
            var besyiloncesi = d2.AddYears(-5);
            var vizesigelenler = (from s in _sicilRepository.GetAll()
                join sm in _sicilMeslekRepository.GetAll() on s.Id equals sm.SICILID
                where sm.VIZESURESIBITTAR == besyiloncesi && sm.MESLEKTERKTAR == null
                select new {s.CEPTEL}).ToList();
            if (vizesigelenler.Count > 0)
            {
                foreach (var item in vizesigelenler)
                    if (item.CEPTEL != null)
                    {
                        if (item.CEPTEL.Length == 10)
                        {
                            cepler += UtilityManager.Seo("90" + item.CEPTEL) + ",";
                            sayac++;
                        }
                        else if (item.CEPTEL.Length == 11)
                        {
                            cepler += UtilityManager.Seo("9" + item.CEPTEL) + ",";
                            sayac++;
                        }
                    }

                cepler = cepler.Substring(0, cepler.Length - 1);
                veriler.Add(cepler);
                veriler.Add(sayac.ToString());
                veriler.Add(besyiloncesi.ToShortDateString());
                foreach (var item in vizesigelenler)
                    if (item.CEPTEL != null)
                    {
                        if (item.CEPTEL.Length == 10)
                        {
                            cepler += UtilityManager.Seo("90" + item.CEPTEL) + ",";
                            sayac++;
                        }
                        else if (item.CEPTEL.Length == 11)
                        {
                            cepler += UtilityManager.Seo("9" + item.CEPTEL) + ",";
                            sayac++;
                        }
                    }

                cepler = cepler.Substring(0, cepler.Length - 1);
                veriler.Add(cepler);
                veriler.Add(sayac.ToString());
                veriler.Add(besyiloncesi.ToShortDateString());
                return veriler;
            }

            cepler = "0";
            veriler.Add(cepler);
            veriler.Add("0");
            veriler.Add(besyiloncesi.ToShortDateString());
            return veriler;
        }

        public List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> GetirIlceyeGoreUyeler(int id)
        {
            var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == id && sm.MESLEKTERKTAR == null
                    select new RaporlarAdreseGoreUyelerListDto
                        {SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL})
                .GroupBy(x => x.SicilNo).ToList();
            return liste;
        }

        public List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> Listele(int? ilce, int? mahalle, int? cadde)
        {
            if (ilce != null && mahalle != null && cadde != null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == ilce && sm.ISADRESMAHALLE == mahalle && sm.ISADRESCADSOKBULV == cadde &&
                          sm.MESLEKTERKTAR == null
                    select new RaporlarAdreseGoreUyelerListDto
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2, Meslek = m.MESLEK
                    }).GroupBy(x => x.SicilNo).ToList();
                return liste;
            }

            if (ilce != null && mahalle != null && cadde == null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == ilce && sm.ISADRESMAHALLE == mahalle && sm.MESLEKTERKTAR == null
                    select new RaporlarAdreseGoreUyelerListDto
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2, Meslek = m.MESLEK
                    }).GroupBy(x => x.SicilNo).ToList();
                return liste;
            }

            if (ilce != null && mahalle == null && cadde == null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == ilce && sm.MESLEKTERKTAR == null
                    select new RaporlarAdreseGoreUyelerListDto
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2, Meslek = m.MESLEK
                    }).GroupBy(x => x.SicilNo).ToList();
                return liste;
            }

            return null;
        }

        public object ListeleV2(int? ilce, int? mahalle, int? cadde)
        {
            if (ilce != null && mahalle != null && cadde != null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == ilce && sm.ISADRESMAHALLE == mahalle && sm.ISADRESCADSOKBULV == cadde &&
                          sm.MESLEKTERKTAR == null
                    select new
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2
                    }).ToList();
                return liste;
            }

            if (ilce != null && mahalle != null && cadde == null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == ilce && sm.ISADRESMAHALLE == mahalle && sm.MESLEKTERKTAR == null
                    select new
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2
                    }).ToList();
                return liste;
            }

            if (ilce != null && mahalle == null && cadde == null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.ISADRESILCE == ilce && sm.MESLEKTERKTAR == null
                    select new
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2
                    }).ToList();
                return liste;
            }

            return null;
        }

        public List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> ListeleOdaUye(int? oda)
        {
            if (oda != null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.MESLEKODASI == oda && sm.MESLEKTERKTAR == null
                    select new RaporlarAdreseGoreUyelerListDto
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2, Meslek = m.MESLEK
                    }).GroupBy(x => x.SicilNo).ToList();
                return liste;
            }

            return null;
        }

        public List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> ListeleMeslekUye(int? meslek)
        {
            if (meslek != null)
            {
                var liste = (from sm in _sicilMeslekRepository.GetAll()
                    join s in _sicilRepository.GetAll() on sm.SICILID equals s.Id
                    join mo in _meslekOdasiRepository.GetAll() on sm.MESLEKODASI equals mo.Id
                    join m in _mesleklerRepository.GetAll() on sm.MESLEK equals m.Id
                    where sm.MESLEK == meslek && sm.MESLEKTERKTAR == null
                    select new RaporlarAdreseGoreUyelerListDto
                    {
                        SicilNo = s.SICILNO, AdSoyad = s.ADSOYAD, TCKimlikNo = s.TCKIMLIKNO, CepTel = s.CEPTEL,
                        IsAdres = sm.ISADRES2, Meslek = m.MESLEK
                    }).GroupBy(x => x.SicilNo).ToList();
                return liste;
            }

            return null;
        }

        private static string ReplaceText(string text)
        {
            text = text.Replace("&#220;", "Ü").Replace("&nbsp;", " ").Replace("&#199;", "Ç").Replace("&#214;", "Ö")
                .Replace("<br>", " ");
            return text;
        }

        private static HtmlDocument HtmlDuzenle(string dosya)
        {
            var url = new Uri(@"c:/Downloads/" + dosya + "");
            var client = new WebClient {Encoding = Encoding.GetEncoding("utf-8")};
            var html = client.DownloadString(url);
            var dokuman = new HtmlDocument();
            dokuman.LoadHtml(html);
            return dokuman;
        }

        public List<ComboBoxIdTextDto> GetirTerkNedeni()
        {
            return _meslekTerkNedeniRepository.GetAll().OrderBy(p => p.Id).ToList()
                .Select(p => new ComboBoxIdTextDto {text = p.ACIKLAMA, id = p.Id.ToString()}).ToList();
        }

        public List<ComboBoxIdTextDto> GetirIlceler(int? id)
        {
            var result = (from a in _ilceRepository.GetAll() orderby a.ILCE select a).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.ILCE}).ToList();
            return result;
        }

        private static bool IsNumeric(string numToValidate)
        {
            if (numToValidate != null)
            {
                var numTest = new double();
                var cultureInfo = new CultureInfo("tr-TR", true);
                return double.TryParse(numToValidate, NumberStyles.Any, cultureInfo.NumberFormat, out numTest);
            }

            return false;
        }

        private int DurumuGetir(List<SicilMeslek> sicilMesleks)
        {
            var durum = 0;
            foreach (var sicilmeslek in sicilMesleks)
                if (sicilmeslek.MESLEKTERKTAR == null)
                {
                    durum = 1;
                    break;
                }

            return durum;
        }

        public Nace Nace(int? id)
        {
            var nace = _naceRepository.GetAll().FirstOrDefault(n => n.Id == id);
            return nace;
        }

        public Meslekler Meslek(int? id)
        {
            var meslek = _mesleklerRepository.GetAll().FirstOrDefault(m => m.Id == id);
            return meslek;
        }
    }
}