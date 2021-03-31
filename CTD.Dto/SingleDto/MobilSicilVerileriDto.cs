using System.Collections.Generic;
using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class MobilSicilVerileriDto
    {
        public List<Kullanici> kullanicilar { get; set; }
        public List<SicilVerileriDto> veriler { get; set; }
    }
}