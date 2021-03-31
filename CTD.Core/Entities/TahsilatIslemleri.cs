using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLTAHSILATISLEMLERI")]
    public class TahsilatIslemleri : BaseEntity
    {
        public int? SICILMESLEKID { get; set; }
        public int SICILID { get; set; }
        public int? ISLEMTURUID { get; set; }
        public int? USERID { get; set; }
        public DateTime? ISLEMTARIHI { get; set; }
        public string MESLEK { get; set; }
        public string ISLEMTURU { get; set; }
        public string USER { get; set; }
    }
}