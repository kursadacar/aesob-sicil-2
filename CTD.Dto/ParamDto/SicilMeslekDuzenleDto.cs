﻿using System;
using System.Collections.Generic;
using CTD.Dto.SingleDto;

namespace CTD.Dto.ParamDto
{
    public class SicilMeslekDuzenleDto
    {
        public int? Id { get; set; }
        public int? SicilId { get; set; }
        public int? IsAdresIlce { get; set; }
        public int? IsAdresMahalle { get; set; }
        public int? IsAdresCadSokBulv { get; set; }
        public int? MeslekOdasi { get; set; }
        public int? Sinif { get; set; }
        public int? MeslekTerkNedeni { get; set; }
        public string IsAdres { get; set; }
        public string IsAdres2 { get; set; }
        public string NaceKodu { get; set; }
        public int? NaceId { get; set; }
        public string NaceTanimi { get; set; }
        public string MeslekKodu { get; set; }
        public string MeslekTanimi { get; set; }
        public int? MeslekId { get; set; }
        public string IsyeriUnvani { get; set; }
        public DateTime? KayitTar { get; set; }
        public DateTime? SonDegisTar { get; set; }
        public DateTime? SonVizeIsTar { get; set; }
        public DateTime? VizeSuresiBitTar { get; set; }
        public DateTime? MeslekTerkTar { get; set; }
        public List<ComboBoxIdTextDto> Ilces { get; set; }
        public List<ComboBoxIdTextDto> Mahalles { get; set; }
        public List<ComboBoxIdTextDto> CadSokBulvs { get; set; }
        public List<ComboBoxIdTextDto> MeslekOdasis { get; set; }
        public List<ComboBoxIdTextDto> Sinifs { get; set; }
        public List<ComboBoxIdTextDto> MeslekTerkNedenis { get; set; }
        public string MeslekOdasiString { get; set; }
        public string NaceKoduString { get; set; }
        public string Aciklama { get; set; }
    }
}