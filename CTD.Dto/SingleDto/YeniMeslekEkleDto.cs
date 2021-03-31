﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CTD.Dto.SingleDto
{
    public class YeniMeslekEkleDto
    {
        public int newsicilid { get; set; }
        [Required] public int newnacekoduid { get; set; }
        public int newmeslekodasi { get; set; }
        public int newmeslekno { get; set; }
        public int newsinif { get; set; }
        public string newunvan { get; set; }
        public int newilce { get; set; }
        public int newmahalle { get; set; }
        public int newcadde { get; set; }
        public string newadres { get; set; }
        [Required] public DateTime newkayittarihi2 { get; set; }
        [Required] public DateTime newvizetarihi2 { get; set; }
    }
}