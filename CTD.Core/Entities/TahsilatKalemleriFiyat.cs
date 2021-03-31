using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLTAHSILATKALEMLERIFIYAT")]
    public class TahsilatKalemleriFiyat : BaseEntity
    {
        public int TAHSILATKALEMID { get; set; }
        public decimal FIYAT { get; set; }
        public int SINIF { get; set; }
    }
}