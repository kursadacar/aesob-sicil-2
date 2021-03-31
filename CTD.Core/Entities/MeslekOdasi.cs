using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("MESLEKODASI")]
    public class MeslekOdasi : BaseEntity
    {
        public string KISAAD { get; set; }
        public string ACIKLAMA { get; set; }
        public int? SINIF { get; set; }
        public string ADRES { get; set; }
        public string TEL { get; set; }
        public bool DURUM { get; set; }
    }
}