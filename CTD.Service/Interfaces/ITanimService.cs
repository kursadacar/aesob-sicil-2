﻿using System.Collections.Generic;
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
        List<CadSokBulv> MahalleyeGoreCadSoklar(int? mahalleid);
        List<MeslekOdasi> GetirOdalar();
        MeslekOdasi GetirOda(int id);
        void MeslekOdasiGuncelle(MeslekOdasi model);
        List<MeslekTerkNedeni> GetirTerkNedenleri();
        MeslekTerkNedeni GetirTerkNedeni(int id);
        void TerkNedenikaydet(MeslekTerkNedeni model);
        List<ComboBoxIdTextDto> ComboBoxTumIlceler();
        void KaydetMahalle(Mahalle model);
        List<ComboBoxIdTextDto> ComboBoxIlceyeGoreMahalleler(int ilceid);
        void KaydetSokak(CadSokBulv model);
        Meslekler GetirMeslek(int meslekid);
        Nace GetirNace(int naceid);
        void KaydetMeslek(Meslekler model);
        void KaydetNace(Nace model);
        List<ComboBoxIdTextDto> GetirMeslekler();
    }
}