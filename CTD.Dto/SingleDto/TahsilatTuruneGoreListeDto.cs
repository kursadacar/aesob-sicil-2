using System;

namespace CTD.Dto.SingleDto
{
    public class TahsilatTuruneGoreListeDto
    {
        public bool? SICILMAKBUZ { get; set; }
        public bool? EVRAKMAKBUZ { get; set; }
        public bool? DIGERMAKBUZ { get; set; }
        public string SERINO { get; set; }
        public int? MAKBUZNO { get; set; }
        public DateTime? MAKBUZTAR { get; set; }
        public string ACIKLAMA { get; set; }
        public decimal? BIRLIKTAHSILATI { get; set; }
        public decimal? IDTAHSILATI { get; set; }
        public bool? IPTAL { get; set; }
        public int? SICILNO { get; set; }
        public string oda { get; set; }
        public string adsoyad { get; set; }
    }
}