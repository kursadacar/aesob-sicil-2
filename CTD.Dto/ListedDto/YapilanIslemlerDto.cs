﻿using System;

namespace CTD.Dto.ListedDto
{
    public class YapilanIslemlerDto
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string IslemTuruAdi { get; set; }
        public DateTime IslemTarihi { get; set; }
    }
}