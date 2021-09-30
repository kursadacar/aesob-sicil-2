using System.Collections.Generic;
using System.Linq;
using CTD.Core.Entities;
using CTD.Dto.ListedDto;
using CTD.Dto.ParamDto;
using CTD.Dto.SingleDto;

namespace CTD.Service.Interfaces
{
    public interface ISicilService
    {
        void InsertSicil(Sicil entity);
        void InsertSicilMeslek(SicilMeslek entity);
        void DeleteSicilMeslek(SicilMeslek entity);
        SicilSingleDto VerileriGetir(string dosya);
        Sicil VerileriGetirKisisel(string dosya);
        int VerileriGetirOda(string dosya);
        string CheckSicilNo(int sicilno);
        string CheckTcKimlik(string tcno);
        List<ComboBoxIdTextDto> GetirSiniflar();
        List<ComboBoxIdTextDto> GetirOdalar();
        List<ComboBoxIdTextDto> GetirMeslekler();
        List<ComboBoxIdTextDto> GetirIlceler();
        string GetirOda(int oda);
        string GetirMeslek(int? id);
        Ilce GetirIlce(int? id);
        Mahalle GetirMahalle(int? id);
        CadSokBulv GetirCadSokBulv(int? id);
        Nace GetirMeslekKodu(string id);
        List<ComboBoxIdTextDto> GetirIlceyeGoreMahalle(int? id);
        List<ComboBoxIdTextDto> GetirMahalleyeGoreCadSokBulv(int? id);
        List<SicilAramaDto> GetirUyeler(string arama);
        SicilDetailDto SicilDetail(int sicilno);
        void SicilGuncelle(Sicil entity);
        Sicil SicilGetir(int? id);
        SicilDto SicilDtoGetir(int? id);
        void KaydetSicilMeslekDegisiklik_log(SicilMeslekDegisiklik_Log model);
        List<SicilMeslekDto> SicilMeslekListesi(int? id);
        SicilDetailMeslekDto SicilDetailMeslek(int? id);
        SicilMeslekDuzenleDto SicilMeslekDuzenle(int? id);
        void UpdateSicilMeslekRecord(SicilMeslek entity);
        SicilMeslek SicilMeslekGetir(int? id);
        NaceMeslekParamDto NaceBilgileriGetir(string nacekodu);
        Nace NaceBilgileriGetir2(int nacekodu);
        SicilMeslekDuzenleDto MerkezBilgileriniGetir(int? id);
        YeniMeslekDto YeniMeslekCombolariGetir();
        SicilSgkYazismaDto SicilSgkBilgi(int id);
        List<YazismaReportList> YazismaReportLists(int sicilid);
        string SicilNoKontrol(int? sicilno);
        List<UnvanAramaDto> UnvanArama(string kriter);
        List<SonSicilMeslekListesi> SonSicilMeslekListe(int sicilid);
        List<SonSicilMeslekDegisiklik_LogListesi> SonSicilMeslekDegisiklikListe(int sicilid);
        void SilSicilMeslekDegisiklik_Log(List<SicilMeslekDegisiklik_Log> model);
        List<SicilMeslekDegisiklik_Log> GetirSicilMeslekDegisiklik_Log(int sicilid);
        SicilMeslekDegisiklik_Log GetirSicilMeslekDegisiklik_Log(int sicilid, int sicilmeslekid);
        int GetirSicilMeslekDegisiklik_LogSayisi(int sicilid);
        List<SicilMeslek> SicilMeslekMevcutDurum(int sicilid);
        List<SicilAramaDto> DetayliArama(string adsoyad, string anneadi, string babaadi, string meslekodasi, string meslek);
        void InsertTahsilatIslemleri(TahsilatIslemleriDto model);
        List<string> VizesiGelenler();
        List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> GetirIlceyeGoreUyeler(int id);
        List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> Listele(int? ilce, int? mahalle, int? cadde);
        object ListeleV2(int? ilce, int? mahalle, int? cadde);
        List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> ListeleOdaUye(int? oda);
        List<IGrouping<int, RaporlarAdreseGoreUyelerListDto>> ListeleMeslekUye(int? meslek);
    }
}