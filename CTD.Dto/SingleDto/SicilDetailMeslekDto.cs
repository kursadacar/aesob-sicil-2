﻿using System;

namespace CTD.Dto.SingleDto
{
    public class SicilDetailMeslekDto
    {
        public int Id { get; set; }
        public int? SicilId { get; set; }
        public string IsyeriUnvani { get; set; }
        public string EskiMeslek { get; set; }
        public bool? Merkez { get; set; }
        public string OdaKisaAd { get; set; }
        public string SinifString { get; set; }
        public string MeslekAdi { get; set; }
        public string MeslekKodu { get; set; }
        public string IsAdres2 { get; set; }
        public string NaceAltiliKod { get; set; }
        public string NaceTanim { get; set; }
        public string MeslekTerkNedeniString { get; set; }
        public string UserString { get; set; }
        public DateTime? VizeSuresiBitTar { get; set; }
        public DateTime? MeslekTerkTar { get; set; }
        public DateTime? KayitTar { get; set; }
        public DateTime? SonVizeIsTar { get; set; }
        public DateTime? SonDegisTar { get; set; }
        public string Aciklama { get; set; }
        public string KayitEden { get; set; }
    }
}