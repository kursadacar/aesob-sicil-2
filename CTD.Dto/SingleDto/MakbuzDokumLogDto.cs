namespace CTD.Dto.SingleDto
{
    public class MakbuzDokumLogDto
    {
        public int Id { get; set; }
        public bool? SicilMakbuz { get; set; }
        public bool? EvrakMakbuz { get; set; }
        public bool? DigerMakbuz { get; set; }
        public int? SicilNo { get; set; }
        public int? Oda { get; set; }
        public string SeriNo { get; set; }
        public int? MakbuzNo { get; set; }
        public string MakbuzTarihi { get; set; }
        public int? KayitEden { get; set; }
        public string KayitTarihi { get; set; }
        public int? IslemiYapan { get; set; }
        public string AdiSoyadi { get; set; }
    }
}