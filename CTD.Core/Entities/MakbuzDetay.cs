using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLMAKBUZDETAY")]
    public class MakbuzDetay : BaseEntity
    {
        public int MAKBUZDOKUMID { get; set; }
        public string KOD { get; set; }
        public string ACIKLAMA { get; set; }
        public decimal TUTAR { get; set; }
        public DateTime? TAHAKKUKTAR { get; set; }
        public int TAHAKKUKETTIREN { get; set; }
        public bool? MAKBUZ { get; set; }
    }
}