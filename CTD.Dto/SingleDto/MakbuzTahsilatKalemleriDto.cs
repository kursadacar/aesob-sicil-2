﻿using System;

namespace CTD.Dto.SingleDto
{
    public class MakbuzTahsilatKalemleriDto
    {
        public int? Id { get; set; }
        public string Kod { get; set; }
        public string TahsilatKalemi { get; set; }
        public bool? Makbuz { get; set; }
        public DateTime? MakbuzTarihi { get; set; }
        public decimal SinifFiyat { get; set; }
    }
}