﻿using System;

namespace CTD.Dto.SingleDto
{
    public class SicilMeslekDto
    {
        public int? ID { get; set; }
        public int? SICILID { get; set; }
        public int? NACEKODU { get; set; }
        public string ISYERIUNVANI { get; set; }
        public int? MESLEKODASI { get; set; }
        public int? MESLEK { get; set; }
        public string ESKI_MESLEK { get; set; }
        public int? ISADRESILCE { get; set; }
        public int? ISADRESMAHALLE { get; set; }
        public int? ISADRESCADSOKBULV { get; set; }
        public int? ISADRESCADSOKBULV2 { get; set; }
        public string ISADRESCADDE { get; set; }
        public string ISADRESSOKAK { get; set; }
        public string ISADRESKAPINO { get; set; }
        public string ISADRESDAIRENO { get; set; }
        public string ESKI_ISADRESMAHALLE { get; set; }
        public string ISADRES_CALISILAN3 { get; set; }
        public string ISADRES_CALISILAN { get; set; }
        public string ISADRES { get; set; }
        public string ISADRES2 { get; set; }
        public int? SINIF { get; set; }
        public DateTime? KAYITTAR { get; set; }
        public DateTime? SONDEGISTAR { get; set; }
        public DateTime? SONVIZEISTAR { get; set; }
        public DateTime? VIZESURESIBITTAR { get; set; }
        public DateTime? MESLEKTERKTAR { get; set; }
        public int MESLEKTERKNEDENI { get; set; }
        public string ESKI_MESLEKTERKNEDENI { get; set; }
        public int? KAYITEDEN { get; set; }
        public DateTime? KAYITEDENTAR { get; set; }
        public int? DEGEDEN { get; set; }
        public DateTime? DEGTAR { get; set; }
        public string ACIKLAMA { get; set; }
        public bool? MERKEZ { get; set; }
        public int? MERKEZID { get; set; }
        public string MESLEKODASIKISAAD { get; set; }
        public string MESLEKADI { get; set; }
    }
}