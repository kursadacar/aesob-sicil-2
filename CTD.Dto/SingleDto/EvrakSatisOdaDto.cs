﻿using System.Collections.Generic;

namespace CTD.Dto.SingleDto
{
    public class EvrakSatisOdaDto
    {
        public List<ComboBoxIdTextDto> MeslekOdalari { get; set; }
        public List<ComboBoxIdTextDto> TahsilatIslemleri { get; set; }
        public MakbuzDto MakbuzBilgisi { get; set; }
    }
}