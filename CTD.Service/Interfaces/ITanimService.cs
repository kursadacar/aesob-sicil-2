using System.Collections.Generic;
using CTD.Core.Entities;
using CTD.Dto.SingleDto;

namespace CTD.Service.Interfaces
{
    public interface ITanimService
    {
        List<Meslekler> TumMeslekler();
        List<Nace> GetirNaceler(int? meslekid);
        void EkleNace(Nace entity);
        List<Ilce> TumIlceler();
        List<Mahalle> IlceyeGoreMahalleler(int? ilceid);
        bool MahalleDuzenle(Mahalle mahalle);
        bool MahalleSil(Mahalle mahalle);
        List<CadSokBulv> MahalleyeGoreCadSoklar(int? mahalleid);
        List<MeslekOdasi> GetirOdalar();
        MeslekOdasi GetirOda(int id);
        void MeslekOdasiGuncelle(MeslekOdasi model);
        void MeslekOdasiSil(MeslekOdasi model);
        List<MeslekTerkNedeni> GetirTerkNedenleri();
        MeslekTerkNedeni GetirTerkNedeni(int id);
        void TerkNedeniSil(MeslekTerkNedeni model);
        void TerkNedenikaydet(MeslekTerkNedeni model);
        List<ComboBoxIdTextDto> ComboBoxTumIlceler();
        void KaydetMahalle(Mahalle model);
        List<ComboBoxIdTextDto> ComboBoxIlceyeGoreMahalleler(int ilceid);
        void KaydetSokak(CadSokBulv model);
        bool CadSokBulvDuzenle(CadSokBulv model);
        bool CadSokBulvSil(CadSokBulv model);
        Meslekler GetirMeslek(int meslekid);
        void MeslekSil(Meslekler meslek);
        void MeslekDuzenle(Meslekler meslek);
        Nace GetirNace(int naceid);
        void NaceSil(Nace nace);
        void NaceDuzenle(Nace nace);
        void KaydetMeslek(Meslekler model);
        void KaydetNace(Nace model);
        List<ComboBoxIdTextDto> GetirMeslekler();
    }
}