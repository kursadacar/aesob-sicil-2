using System;

namespace CTD.Dto.SingleDto
{
    public class MakbuzDokumSilDto
    {
        public int Id { get; set; }
        public string SeriNo { get; set; }
        public int? MakbuzNo { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string Aciklama { get; set; }
        public string AdiSoyadi { get; set; }
    }
}