using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLTAHSILATKALEMLERI")]
    public class TahsilatKalemleri : BaseEntity
    {
        public string KOD { get; set; }
        public string TAHSILATKALEMI { get; set; }
        public bool? MAKBUZ { get; set; }
        public int? TAHSILATGRUPID { get; set; }
        public int? TAHSILATTURUID { get; set; }
        public string ISLEV { get; set; }
        public bool? YENIKAYITTAGELSIN { get; set; }
        public bool? DEGISIKLIKDEGELSIN { get; set; }
        public bool? VIZEDEGELSIN { get; set; }
        public bool? TERKDEGELSIN { get; set; }
        public bool? SURET { get; set; }
        public bool? MESLEKILAVE { get; set; }
        public bool? MAK2 { get; set; }
        public bool? ODALIKAYIT { get; set; }
        public bool? ODASIZKAYIT { get; set; }
    }
}