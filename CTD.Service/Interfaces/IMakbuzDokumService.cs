using System;
using System.Collections.Generic;
using CTD.Core.Entities;
using CTD.Dto.ListedDto;
using CTD.Dto.SingleDto;

namespace CTD.Service.Interfaces
{
    public interface IMakbuzDokumService
    {
        List<MakbuzDokum> GetirBugunkuMakbuzHareket();
        List<HomeGunlukIslemAkisi> GunlukIslemAkisi(string hak, int kullanici);
        MakbuzTahsilatModelDto MakbuzDokum(int? sicilno, int? islem, int? sinif, int? userid);
        Makbuz GetirMakbuz(string serino, int userid);
        List<MakbuzTahsilatKalemleriDto> GetirTahsilatKalemleri(int? islem);
        decimal GetirTahsilatKalemiFiyat(int? sinif, int? tahsilatkalemid);
        void MakbuzKaydet(MakbuzDokum model);
        List<MakbuzTahsilatKalemleriDto> TahsilatKalemleri(int? islem, int? sinif);
        void MakbuzDetayKaydet(MakbuzDetay model);
        List<ComboBoxIdTextDto> GetirOdalar();
        List<ComboBoxIdTextDto> GetirTahsilatKalemleri(string islev);
        MakbuzDto GetirMakbuz(int userid);
        void MakbuzDokumLogKaydet(MakbuzDokumLog model);
        void MakbuzDetayLogKaydet(MakbuzDetayLog model);
        List<MakbuzDetayLogDto> GetirMakbuzDetayLogList(int? makbuzdokumid);
        List<MakbuzDetayLog> GetirSilinecekMakbuzDetayLoglari(int makbuzdokumid);
        void MakbuzDetayLogSil(int id);
        MakbuzDokumLog GetirMakbuzDokumLog(int makbuzdokumid);
        int MakbuzDokumKayitiniAktar(int makbuzdokumid);
        int MakbuzDetayKayitlariniAktar(int makbuzdokumid, int yenimakbuzdokumid);
        MakbuzDetayLog GetirMakbuzDetayLog(int id);
        void SilMakbuzDetayLog(MakbuzDetayLog model);
        void SilMakbuzDokumLog(MakbuzDokumLog model);
        List<MakbuzDetay> GetirMakbuzDetay(int? makbuzdokumid);
        string GetirMeslekOdasi(int? oda);
        void BagisMakbuzDokumEkle(MakbuzDokum model);
        void KiraMakbuzDokumEkle(MakbuzDokum model);
        void BagisMakbuzDetayEkle(MakbuzDetay model);
        void KiraMakbuzDetayEkle(MakbuzDetay model);
        MakbuzDokum GetirMakbuzDokum(string serino, int makbuzno, int kullanici, DateTime makbuztarihi);
        MakbuzDokum GetirMakbuzDokum(int? id);
        MakbuzDetay GetirMakbuzDetayi(int id);
        void MakbuzDetaySil(MakbuzDetay model);
        void MakbuzDetayGuncelle(MakbuzDetay model);
        void MakbuzDokumGuncelle(MakbuzDokum model);
        List<MakbuzDokumSilDto> GetirSilinecekMakbuzlar(string serino, int makbuzno, DateTime makbuztarihi);
        void MakbuzDokumSil(int makbuzid);
        List<ComboBoxIdTextDto> GetirVezneler(int id);
        List<MakbuzDokum> TahsilatTuruneGoreListe(DateTime ilktarih, DateTime sontarih, int kayiteden);
        List<TahsilatTuruneGoreListeDto> TahsilatTuruneGoreListe2(DateTime ilktarih, DateTime sontarih, int kayiteden);
        List<TahsilatTuruneGoreListeDto> TahsilatTuruneGoreIDListe(DateTime ilktarih, DateTime sontarih, int kayiteden);
        List<TahsilatTuruneGoreListeDto> TahsilatTuruneGoreListeV2(DateTime ilktarih, DateTime sontarih, int kayiteden);
        List<MakbuzDetayGelirTablosuDto> MakbuzGelirTablosu(DateTime ilktarih, DateTime sontarih, int vezne);
        int MakbuzGelirTablosuMakbuzSayisi(DateTime ilktarih, DateTime sontarih, int vezne);
        decimal? ToplamBirlikTahsilati(DateTime ilktarih, DateTime sontarih, int vezne);
        string GetirKullanici(int id);
        EskiMakbuzGetirDto MakbuzKontrol(string SeriNo, int MakbuzNo, DateTime MakbuzTarihi);
        List<SicilVerileriDto> SicilVerileri();
        List<TahsilatTurleri> GetirTahsilatTurleri();
        List<ComboBoxIdTextDto> GetirTahsilatTurleriComboBox();
        List<SicilVerileriDto> GetirKullaniciIslemSayilari(DateTime ilktarih, DateTime sontarih, int id);
        List<SicilVerileriDto> GetirKullaniciIslemSayilari(DateTime ilktarih, DateTime sontarih);
        List<YeniKayitYapilanUyelerDto> YeniKayitYapilanUyeler(DateTime ilktarih, DateTime sontarih, int vezne);
    }
}