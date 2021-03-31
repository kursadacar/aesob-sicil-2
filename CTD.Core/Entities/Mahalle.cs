using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLMAHALLE")]
    public class Mahalle : BaseEntity
    {
        public int ILID { get; set; }
        public int ILCEID { get; set; }
        public string MAHALLE { get; set; }
    }
}