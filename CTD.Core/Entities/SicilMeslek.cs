using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTD.Core.Entities
{
    [Table("SICILMESLEK")]
    public class SicilMeslek : BaseEntity
    {
        public int SICILID { get; set; }

        [Required(ErrorMessage = "Nace Kodu girmelisiniz.")]
        public int? NACEKODU { get; set; }

        public string ISYERIUNVANI { get; set; }

        [Required(ErrorMessage = "Meslek Odası seçmelisiniz.")]
        public int? MESLEKODASI { get; set; }

        public int? MESLEK { get; set; }
        public string ESKI_MESLEK { get; set; }

        [Required(ErrorMessage = "İlçe seçmelisiniz.")]
        public int? ISADRESILCE { get; set; }

        [Required] [Display(Name = "MAHALLE")] public int? ISADRESMAHALLE { get; set; }

        [Required(ErrorMessage = "Cadde/Sokak/Bulvar seçmelisiniz.")]
        public int? ISADRESCADSOKBULV { get; set; }

        public int? ISADRESCADSOKBULV2 { get; set; }
        public string ISADRESCADDE { get; set; }
        public string ISADRESSOKAK { get; set; }
        public string ISADRESKAPINO { get; set; }
        public string ISADRESDAIRENO { get; set; }
        public string ESKI_ISADRESMAHALLE { get; set; }
        public string ISADRES_CALISILAN3 { get; set; }
        public string ISADRES_CALISILAN { get; set; }
        public string ISADRES { get; set; }
        public string ISADRES2 { get; set; }

        [Required(ErrorMessage = "Sınıf seçmelisiniz")]
        public int? SINIF { get; set; }

        public DateTime? KAYITTAR { get; set; }
        public DateTime? SONDEGISTAR { get; set; }
        public DateTime? SONVIZEISTAR { get; set; }
        public DateTime? VIZESURESIBITTAR { get; set; }
        public DateTime? MESLEKTERKTAR { get; set; }
        public int MESLEKTERKNEDENI { get; set; }
        public string ESKI_MESLEKTERKNEDENI { get; set; }
        public int? KAYITEDEN { get; set; }
        public DateTime? KAYITEDENTAR { get; set; }
        public int? DEGEDEN { get; set; }
        public DateTime? DEGTAR { get; set; }
        public string ACIKLAMA { get; set; }
        public bool? MERKEZ { get; set; }
        public int? MERKEZID { get; set; }
    }
}