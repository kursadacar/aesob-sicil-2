using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLTAHSILATGRUPLARI")]
    public class TahsilatGruplari : BaseEntity
    {
        public string GRUPADI { get; set; }
    }
}