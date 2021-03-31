using System.Collections.Generic;
using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class PrintMakbuzDto
    {
        public virtual List<MakbuzDetay> MakbuzKalemleri { get; set; }
        public string OdaTamAdi { get; set; }
        public string MakbuzAciklamasi { get; set; }
        public int? SicilNo { get; set; }
        public string AdSoyad { get; set; }
    }
}