using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("SICILMESLEKLOG")]
    public class SicilMeslekLog : BaseEntity
    {
        public int SICILID { get; set; }
        public int NACEKODU { get; set; }
        public string ISYERIUNVANI { get; set; }
        public int MESLEKODASI { get; set; }
        public int MESLEK { get; set; }
        public int ISADRESILCE { get; set; }
        public int ISADRESMAHALLE { get; set; }
        public int ISADRESCADSOKBULV { get; set; }
        public string ISADRES { get; set; }
        public string ISADRES2 { get; set; }
        public int SINIF { get; set; }
        public DateTime KAYITTAR { get; set; }
        public DateTime SONDEGISTAR { get; set; }
        public DateTime SONVIZEISTAR { get; set; }
        public DateTime VIZESURESIBITTAR { get; set; }
        public DateTime MESLEKTERKTAR { get; set; }
        public int MESLEKTERKNEDENI { get; set; }
        public int KAYITEDEN { get; set; }
        public int DEGEDEN { get; set; }
        public DateTime DEGTAR { get; set; }
        public string ACIKLAMA { get; set; }
        public bool MERKEZ { get; set; }
    }
}