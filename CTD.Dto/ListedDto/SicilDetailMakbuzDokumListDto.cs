using System;

namespace CTD.Dto.ListedDto
{
    public class SicilDetailMakbuzDokumListDto
    {
        public DateTime? MakbuzTarihi { get; set; }
        public string MakbuzNo { get; set; }
        public decimal? Tahsilat { get; set; }
        public string OdaAdi { get; set; }
        public string Aciklama { get; set; }
        public string IslemiYapan { get; set; }
    }
}