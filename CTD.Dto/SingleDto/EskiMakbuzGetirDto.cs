﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class EskiMakbuzGetirDto
    {
        public int? ID { get; set; }
        [Required] public string SeriNo { get; set; }
        [Required] public int MakbuzNo { get; set; }
        [Required] public string MakbuzTarihi { get; set; }
        public MakbuzDokum MakbuzDokum { get; set; }
        public List<MakbuzDetay> MakbuzDetaylari { get; set; }
        public List<ComboBoxIdTextDto> MeslekOdalari { get; set; }
        public List<ComboBoxIdTextDto> Kullanicilar { get; set; }
        public List<ComboBoxIdTextDto> Siniflar { get; set; }
        public List<ComboBoxIdTextDto> Islemler { get; set; }
    }
}