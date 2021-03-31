using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLMESLEKTERKNEDENI")]
    public class MeslekTerkNedeni : BaseEntity
    {
        public string ACIKLAMA { get; set; }
    }
}