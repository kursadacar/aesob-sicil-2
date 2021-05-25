using System;
using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class SicilDto
    {
        public int? Id { get; set; }
        public int? SicilNo { get; set; }
        public string TCKimlikNo { get; set; }
        public string AdSoyad { get; set; }
        public int? DogYerIl { get; set; }
        public string DogYerIlce { get; set; }
        public DateTime? DogTrh { get; set; }
        public string BabaAdi { get; set; }
        public string AnaAdi { get; set; }
        public int? Cinsiyet { get; set; }
        public string CepTel { get; set; }
        public int? KayitEden { get; set; }
        public DateTime? KayitTar { get; set; }
        public int? DegEden { get; set; }
        public DateTime? DegTar { get; set; }
        public string Aciklama { get; set; }
        public Sicil Sicil { get; set; }
    }
}