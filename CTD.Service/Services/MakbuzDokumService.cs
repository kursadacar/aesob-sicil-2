using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using CTD.Core.Entities;
using CTD.Core.Helpers;
using CTD.Data.GenericRawSql;
using CTD.Data.GenericRepository;
using CTD.Data.UnitofWork;
using CTD.Dto.ListedDto;
using CTD.Dto.SingleDto;
using CTD.Service.Interfaces;

namespace CTD.Service.Services
{
    public class MakbuzDokumService : IMakbuzDokumService
    {
        private readonly IGenericRepository<Kullanici> _kullaniciRepository;
        private readonly IGenericRepository<MakbuzDetayLog> _makbuzDetayLogRepository;
        private readonly IGenericRepository<MakbuzDetay> _makbuzDetayRepository;
        private readonly IGenericRepository<MakbuzDokumLog> _makbuzDokumLogRepository;
        private readonly IGenericRepository<MakbuzDokum> _makbuzDokumRepository;
        private readonly IGenericRepository<Makbuz> _makbuzRepository;
        private readonly IGenericRepository<Meslekler> _mesleklerRepository;
        private readonly IGenericRepository<MeslekOdasi> _meslekOdasiRepository;
        internal readonly IGenericRawSql<object> _objectRawSql;
        private readonly IGenericRepository<SicilMeslek> _sicilMeslekRepository;
        private readonly IGenericRepository<Sicil> _sicilRepository;
        private readonly IGenericRepository<Sinif> _sinifRepository;
        private readonly IGenericRepository<TahsilatKalemleriFiyat> _tahsilatKalemleriFiyatRepository;
        private readonly IGenericRepository<TahsilatKalemleri> _tahsilatKalemleriRepository;
        private readonly IGenericRepository<TahsilatTurleri> _tahsilatTurleriRepository;

        public MakbuzDokumService(IUnitofWork uow)
        {
            _makbuzDokumRepository = uow.GetRepository<MakbuzDokum>();
            _makbuzDetayRepository = uow.GetRepository<MakbuzDetay>();
            _makbuzDokumLogRepository = uow.GetRepository<MakbuzDokumLog>();
            _makbuzDetayLogRepository = uow.GetRepository<MakbuzDetayLog>();
            _makbuzRepository = uow.GetRepository<Makbuz>();
            _tahsilatTurleriRepository = uow.GetRepository<TahsilatTurleri>();
            _kullaniciRepository = uow.GetRepository<Kullanici>();
            _meslekOdasiRepository = uow.GetRepository<MeslekOdasi>();
            _sicilRepository = uow.GetRepository<Sicil>();
            _sinifRepository = uow.GetRepository<Sinif>();
            _sicilMeslekRepository = uow.GetRepository<SicilMeslek>();
            _tahsilatKalemleriRepository = uow.GetRepository<TahsilatKalemleri>();
            _tahsilatKalemleriFiyatRepository = uow.GetRepository<TahsilatKalemleriFiyat>();
            _mesleklerRepository = uow.GetRepository<Meslekler>();
            _objectRawSql = uow.GetRawSql<object>();
        }

        public List<MakbuzDokum> GetirBugunkuMakbuzHareket()
        {
            return _makbuzDokumRepository.GetAll()
                .Where(c => DbFunctions.TruncateTime(c.MAKBUZTAR) == DbFunctions.TruncateTime(DateTime.Now))
                .OrderByDescending(d => d.Id).ToList();
        }

        public List<HomeGunlukIslemAkisi> GunlukIslemAkisi(string hak, int kullanici)
        {
            if (hak == "mudur" || hak == "admin") //TODO: Recheck
            {
                var list = (from c in _makbuzDokumRepository.GetAll()
                    join tt in _tahsilatTurleriRepository.GetAll() on c.ISLEM equals tt.Id
                    join k in _kullaniciRepository.GetAll() on c.ISLEMIYAPAN equals k.Id
                    where c.MAKBUZTAR == DbFunctions.TruncateTime(DateTime.Now)
                    select new HomeGunlukIslemAkisi
                    {
                        Id = c.Id, AdSoyad = c.ADISOYADI, SicilNo = c.SICILNO.ToString(), Aciklama = c.ACIKLAMA,
                        Islem = c.ISLEM, IslemiYapan = k.adi, IslemTuru = tt.TAHSILATTURU, SicilMakbuz = c.SICILMAKBUZ,
                        EvrakMakbuz = c.EVRAKMAKBUZ, DigerMakbuz = c.DIGERMAKBUZ, Oda = c.ODA, Iptal = c.IPTAL
                    }).OrderByDescending(s => s.Id).ToList();
                return list;
            }
            else
            {
                var list = (from c in _makbuzDokumRepository.GetAll()
                    join tt in _tahsilatTurleriRepository.GetAll() on c.ISLEM equals tt.Id
                    join k in _kullaniciRepository.GetAll() on c.ISLEMIYAPAN equals k.Id
                    where c.MAKBUZTAR == DbFunctions.TruncateTime(DateTime.Now) && c.ISLEMIYAPAN == kullanici
                    select new HomeGunlukIslemAkisi
                    {
                        Id = c.Id, AdSoyad = c.ADISOYADI, SicilNo = c.SICILNO.ToString(), Aciklama = c.ACIKLAMA,
                        Islem = c.ISLEM, IslemiYapan = k.adi, IslemTuru = tt.TAHSILATTURU, SicilMakbuz = c.SICILMAKBUZ,
                        EvrakMakbuz = c.EVRAKMAKBUZ, DigerMakbuz = c.DIGERMAKBUZ, Oda = c.ODA
                    }).OrderByDescending(s => s.Id).ToList();
                return list;
            }
        }

        public MakbuzTahsilatModelDto MakbuzDokum(int? sicilno, int? islem, int? sinif, int? userid)
        {
            var tm = new MakbuzTahsilatModelDto();
            if (islem != null && sinif != null)
            {
                tm.Makbuz = GetirMakbuz(islem, userid);
                tm.UyeMeslekOdaList = GetirSicilMeslek(sicilno);
                tm.AdSoyad = GetirAdSoyad(sicilno);
                if (islem == 2018)
                {
                    var uye = _sicilRepository.GetAll().FirstOrDefault(s => s.SICILNO == sicilno);

                    if (uye == null)
                        return tm;

                    var maxid = _sicilMeslekRepository.GetAll().Where(sm => sm.SICILID == uye.Id).Max(d => d.Id);
                    var sonsicilmeslek = _sicilMeslekRepository.GetAll().FirstOrDefault(f => f.Id == maxid);

                    if (sonsicilmeslek != null)
                        tm.Kullanicilar = GetirKullanici(sonsicilmeslek.KAYITEDEN);

                    if (sonsicilmeslek == null)
                        return tm;

                    //tm.KullaniciId = sonsicilmeslek.KAYITEDEN;
                    tm.MeslekOdasiId = sonsicilmeslek.MESLEKODASI;
                }

                var users = _kullaniciRepository.GetAll().Where(x => x.Makbuz == true);
                tm.Kullanicilar = new List<ComboBoxIdTextDto>();
                foreach (var user in users)
                {
                    tm.Kullanicilar.Add(new ComboBoxIdTextDto() { id = user.Id.ToString(), text = user.adi });
                }

                return tm;
            }

            return null;
        }

        public MakbuzDto GetirMakbuz(int userid)
        {
            var makbuz = _makbuzRepository.GetAll().Where(m => m.KULLANICI == userid && m.ONTAKI != "SNL")
                .FirstOrDefault();
            if(makbuz == null)
            {
                return null;
            }
            var mdto = new MakbuzDto {OnTaki = makbuz.ONTAKI, MakbuzNo = makbuz.MAKBUZNO + 1};
            return mdto;
        }

        public Makbuz GetirMakbuz(string serino, int userid)
        {
            return _makbuzRepository.GetAll().SingleOrDefault(m => m.ONTAKI == serino && m.KULLANICI == userid);
        }

        public List<MakbuzTahsilatKalemleriDto> GetirTahsilatKalemleri(int? islem)
        {
            var kalemler = from tk in _tahsilatKalemleriRepository.GetAll()
                           where tk.TAHSILATTURUID == islem
                           select new MakbuzTahsilatKalemleriDto
                           { Id = tk.Id, Kod = tk.KOD.ToString(), Makbuz = tk.MAKBUZ, TahsilatKalemi = tk.TAHSILATKALEMI };
            
            return kalemler.OrderByDescending(d => d.Makbuz).ToList();
        }

        public decimal GetirTahsilatKalemiFiyat(int? sinif, int? tahsilatkalemid)
        {
            var fiyatObj = _tahsilatKalemleriFiyatRepository.GetAll()
                .SingleOrDefault(c => c.SINIF == sinif && c.TAHSILATKALEMID == tahsilatkalemid);
            return fiyatObj?.FIYAT ?? 0;
        }

        public void MakbuzKaydet(MakbuzDokum model)
        {
            _makbuzDokumRepository.Insert(model);
        }

        public List<MakbuzTahsilatKalemleriDto> TahsilatKalemleri(int? islem, int? sinif)
        {
            if (islem != null && sinif != null)
            {
                var list = GetirTahsilatKalemleri(islem);
                foreach (var item in list)
                    item.SinifFiyat = Convert.ToDecimal(GetirTahsilatKalemiFiyat(sinif, item.Id));
                return list;
            }

            return null;
        }

        public void MakbuzDetayKaydet(MakbuzDetay model)
        {
            _makbuzDetayRepository.Insert(model);
        }

        public List<ComboBoxIdTextDto> GetirOdalar()
        {
            var result = (from c in _meslekOdasiRepository.GetAll() where c.DURUM orderby c.KISAAD select c).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.KISAAD}).ToList();
            return result;
        }

        public List<ComboBoxIdTextDto> GetirTahsilatTurleriComboBox()
        {
            var result = (from c in _tahsilatTurleriRepository.GetAll() orderby c.Id select c).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.TAHSILATTURU}).ToList();
            return result;
        }

        public List<ComboBoxIdTextDto> GetirTahsilatKalemleri(string islev)
        {
            var result =
                (from c in _tahsilatKalemleriRepository.GetAll()
                    where c.ISLEV == islev
                    orderby c.TAHSILATKALEMI
                    select c).ToList().Select(p => new ComboBoxIdTextDto
                    {id = p.KOD.ToString(), text = p.TAHSILATKALEMI}).ToList();
            return result;
        }

        public void MakbuzDokumLogKaydet(MakbuzDokumLog model)
        {
            _makbuzDokumLogRepository.Insert(model);
        }

        public void MakbuzDetayLogKaydet(MakbuzDetayLog model)
        {
            _makbuzDetayLogRepository.Insert(model);
        }

        public List<MakbuzDetayLogDto> GetirMakbuzDetayLogList(int? makbuzdokumid)
        {
            return (from tk in _makbuzDetayLogRepository.GetAll()
                where tk.MAKBUZDOKUMID == makbuzdokumid
                select new MakbuzDetayLogDto
                {
                    Id = tk.Id, MakbuzDokumId = tk.MAKBUZDOKUMID, Kod = tk.KOD, Aciklama = tk.ACIKLAMA,
                    Tutar = tk.TUTAR, Makbuz = tk.MAKBUZ
                }).OrderBy(d => d.Id).ToList();
        }

        public List<MakbuzDetayLog> GetirSilinecekMakbuzDetayLoglari(int makbuzdokumid)
        {
            return (from tk in _makbuzDetayLogRepository.GetAll() where tk.MAKBUZDOKUMID == makbuzdokumid select tk)
                .OrderBy(d => d.Id).ToList();
        }

        public void MakbuzDetayLogSil(int id)
        {
            var mdl = _makbuzDetayLogRepository.Find(id);
            _makbuzDetayLogRepository.Delete(mdl);
        }

        public MakbuzDokumLog GetirMakbuzDokumLog(int makbuzdokumid)
        {
            var mdl = new MakbuzDokumLog();
            mdl = _makbuzDokumLogRepository.Find(makbuzdokumid);
            return mdl;
        }

        public int MakbuzDokumKayitiniAktar(int makbuzdokumid)
        {
            var connStr = ConfigurationManager.ConnectionStrings["CtdEntities"].ConnectionString;
            var yeniid = 0;
            var result = _objectRawSql.ExecuteSqlCommand(connStr,
                "Insert into TBLMAKBUZDOKUM(SICILMAKBUZ, EVRAKMAKBUZ, DIGERMAKBUZ, SICILNO, ODA, TOPLAMTAHSILAT, BIRLIKTAHSILATI, SERINO, MAKBUZNO, MAKBUZTAR, ACIKLAMA, KAYITEDEN, KAYITTAR, ISLEMIYAPAN, SUBE, ISLEM, ADISOYADI) select SICILMAKBUZ, EVRAKMAKBUZ, DIGERMAKBUZ, SICILNO, ODA, TOPLAMTAHSILAT, BIRLIKTAHSILATI, SERINO, MAKBUZNO, MAKBUZTAR, ACIKLAMA, KAYITEDEN, KAYITTAR, ISLEMIYAPAN, SUBE, ISLEM, ADISOYADI From TBLMAKBUZDOKUMLOG Where TBLMAKBUZDOKUMLOG.ID = {0}",
                makbuzdokumid);
            if (result == 1) yeniid = _makbuzDokumRepository.GetAll().Max(a => a.Id);
            return yeniid;
        }

        public int MakbuzDetayKayitlariniAktar(int makbuzdokumid, int yenimakbuzdokumid)
        {
            var connStr = ConfigurationManager.ConnectionStrings["CtdEntities"].ConnectionString;
            var yeniid = 0;
            var result = _objectRawSql.ExecuteSqlCommand(connStr,
                "Insert into TBLMAKBUZDETAY(MAKBUZDOKUMID, KOD, ACIKLAMA, TUTAR, TAHAKKUKTAR, TAHAKKUKETTIREN, MAKBUZ) select {0}, KOD, ACIKLAMA, TUTAR, TAHAKKUKTAR, TAHAKKUKETTIREN, MAKBUZ From TBLMAKBUZDETAYLOG Where TBLMAKBUZDETAYLOG.MAKBUZDOKUMID = {1}",
                yenimakbuzdokumid, makbuzdokumid);
            if (result == 1) yeniid = _makbuzDokumRepository.GetAll().Max(a => a.Id);
            return yeniid;
        }

        public MakbuzDetayLog GetirMakbuzDetayLog(int id)
        {
            var mdl = _makbuzDetayLogRepository.GetAll().SingleOrDefault(s => s.Id == id);
            return mdl;
        }

        public void SilMakbuzDetayLog(MakbuzDetayLog model)
        {
            _makbuzDetayLogRepository.Delete(model);
        }

        public void SilMakbuzDokumLog(MakbuzDokumLog model)
        {
            _makbuzDokumLogRepository.Delete(model);
        }

        public List<MakbuzDetay> GetirMakbuzDetay(int? makbuzdokumid)
        {
            return (from tk in _makbuzDetayRepository.GetAll() where tk.MAKBUZDOKUMID == makbuzdokumid select tk)
                .OrderBy(d => d.Id).ToList();
        }

        public string GetirMeslekOdasi(int? oda)
        {
            var odatamadi = _meslekOdasiRepository.GetAll().Where(o => o.Id == oda).FirstOrDefault().ACIKLAMA;
            return odatamadi;
        }

        public void BagisMakbuzDokumEkle(MakbuzDokum model)
        {
            _makbuzDokumRepository.Insert(model);
        }

        public void BagisMakbuzDetayEkle(MakbuzDetay model)
        {
            _makbuzDetayRepository.Insert(model);
        }

        public void KiraMakbuzDokumEkle(MakbuzDokum model)
        {
            _makbuzDokumRepository.Insert(model);
        }

        public void KiraMakbuzDetayEkle(MakbuzDetay model)
        {
            _makbuzDetayRepository.Insert(model);
        }

        public MakbuzDokum GetirMakbuzDokum(string serino, int makbuzno, int kullanici, DateTime makbuztarihi, out string exception)
        {
            exception = null;

            var mk = _makbuzDokumRepository.GetAll().SingleOrDefault(c =>
                c.SERINO == serino && c.MAKBUZNO == makbuzno &&
                c.MAKBUZTAR == makbuztarihi);

            //if(mk != null)
            //{
            //    if(mk.KAYITEDEN != kullanici)
            //    {
            //        exception = "wrong-user|" + _kullaniciRepository.GetAll().FirstOrDefault(x => x.Id == mk.KAYITEDEN).adi;
            //        return null;
            //    }
            //}
            //else
            //{
            //    exception = "not-found";
            //}

            return mk;
        }

        public MakbuzDokum GetirMakbuzDokum(int? id)
        {
            var mk = _makbuzDokumRepository.GetAll().SingleOrDefault(c => c.Id == id);
            return mk;
        }

        public MakbuzDetay GetirMakbuzDetayi(int id)
        {
            return _makbuzDetayRepository.Find(id);
        }

        public void MakbuzDetaySil(MakbuzDetay model)
        {
            _makbuzDetayRepository.Delete(model);
        }

        public List<ComboBoxIdTextDto> GetirVezneler(int id)
        {
            var yetki = _kullaniciRepository.GetAll().FirstOrDefault(k => k.Id == id)?.hak;
            if (yetki == "admin" || yetki == "muhasebe") //TODO: Recheck this
            {
                var result =
                    (from c in _kullaniciRepository.GetAll()
                        where c.YETKILI && (c.hak == "sube" || c.hak == "mudur" || c.hak == "admin") //TODO: Recheck
                        select c).ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.adi})
                    .ToList();
                result.Add(new ComboBoxIdTextDto {id = "99", text = "Tüm Vezneler"});
                return result;
            }

            if (yetki == "sube" || yetki == "mudur")
            {
                var result = (from c in _kullaniciRepository.GetAll() where c.YETKILI && c.Id == id select c).ToList()
                    .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.adi}).ToList();
                return result;
            }

            return null;
        }

        public List<MakbuzDokum> TahsilatTuruneGoreListe(DateTime ilktarih, DateTime sontarih, int kayiteden)
        {
            if (kayiteden == 99)
            {
                var liste = (from x in _makbuzDokumRepository.GetAll()
                    where x.MAKBUZTAR >= ilktarih && x.MAKBUZTAR <= sontarih
                    select x).OrderBy(d => d.MAKBUZNO).ToList();
                return liste;
            }
            else
            {
                var liste = (from x in _makbuzDokumRepository.GetAll()
                    where x.MAKBUZTAR >= ilktarih && x.MAKBUZTAR <= sontarih && x.KAYITEDEN == kayiteden
                    select x).OrderBy(d => d.MAKBUZNO).ToList();
                return liste;
            }
        }

        public List<TahsilatTuruneGoreListeDto> TahsilatTuruneGoreListe2(DateTime ilktarih, DateTime sontarih,
            int kayiteden)
        {
            var liste = (from x in _makbuzDokumRepository.GetAll()
                where x.MAKBUZTAR >= ilktarih && x.MAKBUZTAR <= sontarih && x.KAYITEDEN == kayiteden &&
                      x.BIRLIKTAHSILATI > 0
                select new TahsilatTuruneGoreListeDto
                {
                    SICILMAKBUZ = x.SICILMAKBUZ, EVRAKMAKBUZ = x.EVRAKMAKBUZ, DIGERMAKBUZ = x.DIGERMAKBUZ,
                    SERINO = x.SERINO, MAKBUZNO = x.MAKBUZNO, MAKBUZTAR = x.MAKBUZTAR, ACIKLAMA = x.ACIKLAMA,
                    BIRLIKTAHSILATI = x.BIRLIKTAHSILATI, IPTAL = x.IPTAL, SICILNO = x.SICILNO, adsoyad = x.ADISOYADI
                }).ToList();
            return liste;
        }

        public List<TahsilatTuruneGoreListeDto> TahsilatTuruneGoreIDListe(DateTime ilktarih, DateTime sontarih,
            int kayiteden)
        {
            var liste = (from x in _makbuzDokumRepository.GetAll()
                where x.MAKBUZTAR >= ilktarih && x.MAKBUZTAR <= sontarih && x.KAYITEDEN == kayiteden &&
                      x.IDTAHSILATI > 0
                select new TahsilatTuruneGoreListeDto
                {
                    SICILMAKBUZ = x.SICILMAKBUZ, EVRAKMAKBUZ = x.EVRAKMAKBUZ, DIGERMAKBUZ = x.DIGERMAKBUZ,
                    SERINO = x.SERINO, MAKBUZNO = x.MAKBUZNO, MAKBUZTAR = x.MAKBUZTAR, ACIKLAMA = x.ACIKLAMA,
                    IDTAHSILATI = x.IDTAHSILATI, IPTAL = x.IPTAL, SICILNO = x.SICILNO, adsoyad = x.ADISOYADI
                }).OrderBy(d => d.MAKBUZNO).ToList();
            return liste;
        }

        public List<TahsilatTuruneGoreListeDto> TahsilatTuruneGoreListeV2(DateTime ilktarih, DateTime sontarih,
            int kayiteden)
        {
            if (kayiteden == 99)
            {
                var liste = (from x in _makbuzDokumRepository.GetAll()
                    where x.MAKBUZTAR >= ilktarih && x.MAKBUZTAR <= sontarih
                    select new TahsilatTuruneGoreListeDto
                    {
                        SICILMAKBUZ = x.SICILMAKBUZ, EVRAKMAKBUZ = x.EVRAKMAKBUZ, DIGERMAKBUZ = x.DIGERMAKBUZ,
                        SERINO = x.SERINO, MAKBUZNO = x.MAKBUZNO, MAKBUZTAR = x.MAKBUZTAR, ACIKLAMA = x.ACIKLAMA,
                        BIRLIKTAHSILATI = x.BIRLIKTAHSILATI, IDTAHSILATI = x.IDTAHSILATI, IPTAL = x.IPTAL,
                        SICILNO = x.SICILNO, adsoyad = x.ADISOYADI
                    }).OrderBy(d => d.MAKBUZNO).ToList();
                return liste;
            }
            else
            {
                var liste = (from x in _makbuzDokumRepository.GetAll()
                    where x.MAKBUZTAR >= ilktarih && x.MAKBUZTAR <= sontarih && x.KAYITEDEN == kayiteden
                    select new TahsilatTuruneGoreListeDto
                    {
                        SICILMAKBUZ = x.SICILMAKBUZ, EVRAKMAKBUZ = x.EVRAKMAKBUZ, DIGERMAKBUZ = x.DIGERMAKBUZ,
                        SERINO = x.SERINO, MAKBUZNO = x.MAKBUZNO, MAKBUZTAR = x.MAKBUZTAR, ACIKLAMA = x.ACIKLAMA,
                        BIRLIKTAHSILATI = x.BIRLIKTAHSILATI, IDTAHSILATI = x.IDTAHSILATI, IPTAL = x.IPTAL,
                        SICILNO = x.SICILNO, adsoyad = x.ADISOYADI
                    }).OrderBy(d => d.MAKBUZNO).ToList();
                return liste;
            }
        }

        public List<MakbuzDetayGelirTablosuDto> MakbuzGelirTablosu(DateTime ilktarih, DateTime sontarih, int vezne)
        {
            if (vezne == 99)
            {
                var connStr = ConfigurationManager.ConnectionStrings["CtdEntities"].ConnectionString;
                var result = _objectRawSql
                    .Execute(connStr, typeof(MakbuzDetayGelirTablosuDto), "exec spMakbuzGelirTablosu3 {0},{1}",
                        ilktarih, sontarih).Cast<MakbuzDetayGelirTablosuDto>().ToList();
                return result;
            }
            else
            {
                var connStr = ConfigurationManager.ConnectionStrings["CtdEntities"].ConnectionString;
                var result = _objectRawSql
                    .Execute(connStr, typeof(MakbuzDetayGelirTablosuDto), "exec spMakbuzGelirTablosu2 {0},{1},{2}",
                        ilktarih, sontarih, vezne).Cast<MakbuzDetayGelirTablosuDto>().ToList();
                return result;
            }
        }

        public int MakbuzGelirTablosuMakbuzSayisi(DateTime ilktarih, DateTime sontarih, int vezne)
        {
            if (vezne == 99)
            {
                var result = (from c in _makbuzDokumRepository.GetAll()
                    where c.SERINO != "SNL" && c.MAKBUZTAR >= ilktarih && c.MAKBUZTAR <= sontarih
                    select c).Count();
                return result;
            }
            else
            {
                var result = (from c in _makbuzDokumRepository.GetAll()
                    where c.KAYITEDEN == vezne && c.SERINO != "SNL" && c.MAKBUZTAR >= ilktarih &&
                          c.MAKBUZTAR <= sontarih
                    select c).Count();
                return result;
            }
        }

        public decimal? ToplamBirlikTahsilati(DateTime ilktarih, DateTime sontarih, int vezne)
        {
            if (vezne == 99)
            {
                var result = (from c in _makbuzDokumRepository.GetAll()
                    where c.SERINO != "SNL" && c.MAKBUZTAR >= ilktarih && c.MAKBUZTAR <= sontarih
                    select c.BIRLIKTAHSILATI).Sum();
                return result;
            }
            else
            {
                var result = (from c in _makbuzDokumRepository.GetAll()
                    where c.KAYITEDEN == vezne && c.SERINO != "SNL" && c.MAKBUZTAR >= ilktarih &&
                          c.MAKBUZTAR <= sontarih
                    select c.BIRLIKTAHSILATI).Sum();
                return result;
            }
        }

        public string GetirKullanici(int id)
        {
            if (id == 99)
                return "TÜM VEZNELER";
            return _kullaniciRepository.GetAll().FirstOrDefault(k => k.Id == id).adi;
        }

        public EskiMakbuzGetirDto MakbuzKontrol(string SeriNo, int MakbuzNo, DateTime MakbuzTarihi, out string exception)
        {
            exception = null;

            var makbuz = _makbuzDokumRepository.GetAll().FirstOrDefault(m =>
                m.SERINO == SeriNo && m.MAKBUZNO == MakbuzNo && m.MAKBUZTAR == MakbuzTarihi);
            if (makbuz != null)
            {
                var eski = new EskiMakbuzGetirDto
                {
                    ID = makbuz.Id, SeriNo = SeriNo, MakbuzNo = MakbuzNo, MakbuzTarihi = MakbuzTarihi.ToString(),
                    MakbuzDokum = makbuz,
                    MakbuzDetaylari = _makbuzDetayRepository.GetAll()
                        .Where(makbuzdetay => makbuzdetay.MAKBUZDOKUMID == makbuz.Id).ToList(),
                    MeslekOdalari = GetirOdalar(), Kullanicilar = GetirMemurlar(), Siniflar = GetirSiniflar(),
                    Islemler = GetirTahsilatTurleriComboBox()
                };
                return eski;
            }
            else
            {
                exception = "not-found";
            }

            return null;
        }

        public void MakbuzDetayGuncelle(MakbuzDetay model)
        {
            _makbuzDetayRepository.Update(model);
        }

        public void MakbuzDokumGuncelle(MakbuzDokum model)
        {
            _makbuzDokumRepository.Update(model);
        }

        public List<MakbuzDokumSilDto> GetirSilinecekMakbuzlar(string serino, int makbuzno, DateTime makbuztarihi)
        {
            var list = (from c in _makbuzDokumRepository.GetAll()
                where c.SERINO == serino && c.MAKBUZNO == makbuzno &&
                      DbFunctions.TruncateTime(c.MAKBUZTAR) == DbFunctions.TruncateTime(makbuztarihi)
                select new MakbuzDokumSilDto
                {
                    Id = c.Id, AdiSoyadi = c.ADISOYADI, SeriNo = c.SERINO, Aciklama = c.ACIKLAMA, MakbuzNo = c.MAKBUZNO,
                    KayitTarihi = c.KAYITTAR
                }).OrderByDescending(s => s.Id).ToList();
            return list;
        }

        public void MakbuzDokumSil(int makbuzid)
        {
            var makbuzdokum = _makbuzDokumRepository.Find(makbuzid);
            _makbuzDokumRepository.Delete(makbuzdokum);
        }

        public List<SicilVerileriDto> SicilVerileri()
        {
            var list =
                (from makbuzdokum in _makbuzDokumRepository.GetAll().Where(c =>
                        DbFunctions.TruncateTime(c.MAKBUZTAR) == DbFunctions.TruncateTime(DateTime.Now))
                    group makbuzdokum by makbuzdokum.ISLEM
                    into grup
                    select new SicilVerileriDto {Islem = grup.Key.ToString(), Sayi = grup.Count()}).ToList();
            return list;
        }

        public List<TahsilatTurleri> GetirTahsilatTurleri()
        {
            return _tahsilatTurleriRepository.GetAll().OrderBy(c => c.Id).ToList();
        }

        public List<SicilVerileriDto> GetirKullaniciIslemSayilari(DateTime ilktarih, DateTime sontarih, int id)
        {
            var liste = (from detay in _makbuzDokumRepository.GetAll().Where(c =>
                        DbFunctions.TruncateTime(c.MAKBUZTAR) >= DbFunctions.TruncateTime(ilktarih) &&
                        DbFunctions.TruncateTime(c.MAKBUZTAR) <= DbFunctions.TruncateTime(sontarih) &&
                        c.SICILMAKBUZ == true &&
                        c.ISLEMIYAPAN == id)
                    group detay by detay.ISLEM
                    into grup
                    select new SicilVerileriDto {Islem = grup.Key.ToString(), Sayi = grup.Count()})
                .OrderBy(d => d.Islem)
                .ToList();
            return liste;
        }

        public List<SicilVerileriDto> GetirKullaniciIslemSayilari(DateTime ilktarih, DateTime sontarih)
        {
            var liste = (from detay in _makbuzDokumRepository.GetAll().Where(c =>
                        DbFunctions.TruncateTime(c.MAKBUZTAR) >= DbFunctions.TruncateTime(ilktarih) &&
                        DbFunctions.TruncateTime(c.MAKBUZTAR) <= DbFunctions.TruncateTime(sontarih) &&
                        c.SICILMAKBUZ == true)
                    group detay by detay.ISLEM
                    into grup
                    select new SicilVerileriDto {Islem = grup.Key.ToString(), Sayi = grup.Count()})
                .OrderBy(d => d.Islem)
                .ToList();
            return liste;
        }

        public List<YeniKayitYapilanUyelerDto> YeniKayitYapilanUyeler(DateTime ilktarih, DateTime sontarih, int vezne)
        {
            if (vezne == 99)
            {
                var connStr = ConfigurationManager.ConnectionStrings["CtdEntities"].ConnectionString;
                var result = _objectRawSql
                    .Execute(connStr, typeof(YeniKayitYapilanUyelerDto), "exec spYeniKayitYapilanUyeler {0},{1}",
                        ilktarih, sontarih).Cast<YeniKayitYapilanUyelerDto>().ToList();
                return result;
            }
            else
            {
                var connStr = ConfigurationManager.ConnectionStrings["CtdEntities"].ConnectionString;
                var result = _objectRawSql
                    .Execute(connStr, typeof(YeniKayitYapilanUyelerDto), "exec spYeniKayitYapilanUyeler2 {0},{1},{2}",
                        ilktarih, sontarih, vezne).Cast<YeniKayitYapilanUyelerDto>().ToList();
                return result;
            }
        }

        private MakbuzDto GetirMakbuz(int? islem, int? userid)
        {
            var mm = new MakbuzDto();
            var m = new Makbuz();
            mm.KullaniciId = userid;
            m = _makbuzRepository.GetAll().SingleOrDefault(c => c.KULLANICI == userid);
            mm.OnTaki = m.ONTAKI;
            mm.MakbuzNo = m.MAKBUZNO + 1;
            mm.Id = m.Id;
            return mm;
        }

        private List<ComboBoxIdTextDto> GetirSicilMeslek(int? sicilno)
        {
            if (sicilno != null)
            {
                var sicil = _sicilRepository.GetAll().FirstOrDefault(c => c.SICILNO == sicilno);
                if (sicil == null) return null;
                var sicilid = sicil.Id;
                var result =
                    (from a in _sicilMeslekRepository.GetAll()
                        join mo in _meslekOdasiRepository.GetAll() on a.MESLEKODASI equals mo.Id
                        where a.SICILID == sicilid && a.MESLEKTERKTAR == null
                        select mo).ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.KISAAD})
                    .ToList();
                return result;
            }

            return null;
        }

        private string GetirAdSoyad(int? sicilno)
        {
            if (sicilno != null)
            {
                var sicil = _sicilRepository.GetAll().FirstOrDefault(c => c.SICILNO == sicilno);
                var adsoyad = sicil?.ADSOYAD;
                return adsoyad;
            }

            return null;
        }

        private List<ComboBoxIdTextDto> GetirKullanici(int? kullaniciid)
        {
            if (kullaniciid != null)
            {
                var result = (from c in _kullaniciRepository.GetAll() where c.YETKILI && c.Id == kullaniciid select c)
                    .ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.adi}).ToList();
                return result;
            }

            return null;
        }

        private List<ComboBoxIdTextDto> GetirKullanicilar()
        {
            var result = (from c in _kullaniciRepository.GetAll() where c.YETKILI && c.sube == "MERKEZ" select c)
                .ToList().Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.adi}).ToList();
            return result;
        }

        public List<ComboBoxIdTextDto> GetirMemurlar()
        {
            var result = (from c in _kullaniciRepository.GetAll() where c.YETKILI orderby c.adi select c).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.adi}).ToList();
            return result;
        }

        public List<ComboBoxIdTextDto> GetirSiniflar()
        {
            var result = (from c in _sinifRepository.GetAll() orderby c.Id select c).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.SINIF}).ToList();
            return result;
        }
    }
}