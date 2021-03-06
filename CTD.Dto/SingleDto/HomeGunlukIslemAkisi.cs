namespace CTD.Dto.SingleDto
{
    public class HomeGunlukIslemAkisi
    {
        public int? Id { get; set; }
        public string AdSoyad { get; set; }
        public string SicilNo { get; set; }
        public string IslemTuru { get; set; }
        public string IslemiYapan { get; set; }
        public bool? SicilMakbuz { get; set; }
        public bool? EvrakMakbuz { get; set; }
        public bool? DigerMakbuz { get; set; }
        public int? Islem { get; set; }
        public int? Oda { get; set; }
        public string OdaAdi { get; set; }
        public string Aciklama { get; set; }
        public bool? Iptal { get; set; }
    }
}