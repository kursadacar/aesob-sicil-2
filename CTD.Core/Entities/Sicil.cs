using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("SICIL")]
    public class Sicil : BaseEntity
    {
        [Required(ErrorMessage = "Sicil No girmelisiniz.")]
        public int SICILNO { get; set; }

        [Required(ErrorMessage = "TC Kimlik girmelisiniz.")]
        public string TCKIMLIKNO { get; set; }

        [Required(ErrorMessage = "Ad ve Soyad girmelisiniz.")]
        public string ADSOYAD { get; set; }

        public int? DOGYERIL { get; set; }
        public string DOGYERILCE { get; set; }
        public DateTime? DOGTAR { get; set; }
        public string BABAADI { get; set; }
        public string ANAADI { get; set; }
        public int CINSIYET { get; set; }
        public string CEPTEL { get; set; }
        public int KAYITEDEN { get; set; }
        public DateTime? KAYITTAR { get; set; }
        public int? DEGEDEN { get; set; }
        public DateTime? DEGTAR { get; set; }
        public string ACIKLAMA { get; set; }
    }
}