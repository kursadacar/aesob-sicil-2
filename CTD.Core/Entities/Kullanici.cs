using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("TBLUSERS")]
    public class Kullanici : BaseEntity
    {
        public string login { get; set; }
        public string pass { get; set; }
        public string pass2 { get; set; }
        public string adi { get; set; }
        public string sube { get; set; }
        public bool YETKILI { get; set; }
        public string hak { get; set; }
        public string ip { get; set; }
        public string resim { get; set; }
        public bool Makbuz { get; set; }
    }
}