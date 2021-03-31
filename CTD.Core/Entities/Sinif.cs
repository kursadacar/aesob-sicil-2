using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLSINIFLAR")]
    public class Sinif : BaseEntity
    {
        public string SINIF { get; set; }
    }
}