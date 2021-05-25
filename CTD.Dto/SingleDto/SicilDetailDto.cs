using System.Collections.Generic;
using CTD.Core.Entities;
using CTD.Dto.ListedDto;

namespace CTD.Dto.SingleDto
{
    public class SicilDetailDto
    {
        public Sicil Sicil { get; set; }
        public List<SicilMeslek> SicilMeslekler { get; set; }
        public List<SicilDetailMakbuzDokumListDto> MakbuzDokumleri { get; set; }
        public List<TahsilatIslemleriDto> GecmisIslemler { get; set; }
        public int? Durum { get; set; }
    }
}