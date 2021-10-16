using System;

namespace CTD.Dto.SingleDto
{
    public class SonSicilMeslekListesi
    {
        public int? id { get; set; }
        public string adres { get; set; }
        public string cadde { get; set; }
        public string ilce { get; set; }
        public string islemiyapan { get; set; }
        public string isyeriunvani { get; set; }
        public string kayittarihi { get; set; }
        public string islemtarihi { get; set; }
        public string vizetarihi { get; set; }
        public DateTime? islemTarihiDT { get; set; }
        public DateTime? kayittarihiDT { get; set; }
        public DateTime? vizetarihiDT { get; set; }
        public string mahalle { get; set; }
        public string meslek { get; set; }
        public string meslekkodu { get; set; }
        public string nacekodu { get; set; }
        public string odasi { get; set; }
        public int? sinifi { get; set; }
    }
}