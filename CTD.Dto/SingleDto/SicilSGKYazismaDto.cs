using System;

namespace CTD.Dto.SingleDto
{
    public class SicilSgkYazismaDto
    {
        public string AdSoyad { get; set; }
        public string TcKimlikNo { get; set; }
        public string BabaAdi { get; set; }
        public DateTime DogumTarihi { get; set; }
        public DateTime Kayittarihi { get; set; }
        public int Durum { get; set; }
        public int SicilNo { get; set; }
    }
}