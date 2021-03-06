using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLMAKBUZDOKUM")]
    public class MakbuzDokum : BaseEntity
    {
        public bool? SICILMAKBUZ { get; set; }
        public bool? EVRAKMAKBUZ { get; set; }
        public bool? DIGERMAKBUZ { get; set; }
        public int? SICILNO { get; set; }
        public int? ODA { get; set; }
        public decimal? TOPLAMTAHSILAT { get; set; }
        public decimal? IDTAHSILATI { get; set; }
        public decimal? BIRLIKTAHSILATI { get; set; }
        public string SERINO { get; set; }
        public int? MAKBUZNO { get; set; }
        public DateTime? MAKBUZTAR { get; set; }
        public string ACIKLAMA { get; set; }
        public bool? IPTAL { get; set; }
        public DateTime? IPTALTAR { get; set; }
        public int? IPTALEDEN { get; set; }
        public int? KAYITEDEN { get; set; }
        public DateTime? KAYITTAR { get; set; }
        public int? DEGEDEN { get; set; }
        public DateTime? DEGTAR { get; set; }
        public int? ISLEMIYAPAN { get; set; }
        public string SUBE { get; set; }
        public int? ISLEM { get; set; }
        public string ADISOYADI { get; set; }
        public int? SICILMESLEKID { get; set; }
    }
}