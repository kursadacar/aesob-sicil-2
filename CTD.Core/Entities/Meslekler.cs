using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLMESLEKLER")]
    public class Meslekler : BaseEntity
    {
        public int MESLEKODASIID { get; set; }
        public string MESLEKKODU { get; set; }
        public string MESLEK { get; set; }
    }
}