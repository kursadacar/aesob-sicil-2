using System;

namespace CTD.Dto.SingleDto
{
    public class SonSicilMeslekDegisiklik_LogListesi
    {
        public int? Id { get; set; }
        public int? SicilMeslekId { get; set; }
        public string IsyeriUnvani { get; set; }
        public string NaceKodu { get; set; }
        public string MeslekKodu { get; set; }
        public string Meslek { get; set; }
        public int? Sinifi { get; set; }
        public string Odasi { get; set; }
        public DateTime? KayitTarihiDT { get; set; }
        public DateTime? VizeTarihiDT { get; set; }
        public DateTime? IslemTarihiDT { get; set; }
        public string KayitTarihi { get; set; }
        public string VizeTarihi { get; set; }
        public string IslemTarihi { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Adres { get; set; }
        public string Ilce { get; set; }
        public string IslemiYapan { get; set; }
    }
}