﻿namespace CTD.Dto.SingleDto
{
    public class MakbuzDetayLogDto
    {
        public int Id { get; set; }
        public int? MakbuzDokumId { get; set; }
        public string Kod { get; set; }
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public string TahakkukTarihi { get; set; }
        public int? TahakkukEttiren { get; set; }
        public bool? Makbuz { get; set; }
    }
}