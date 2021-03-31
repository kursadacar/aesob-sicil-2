using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLTAHSILATTURLERI")]
    public class TahsilatTurleri : BaseEntity
    {
        public string TAHSILATTURU { get; set; }
    }
}