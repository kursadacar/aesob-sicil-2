using System.Collections.Generic;

namespace CTD.Dto.SingleDto
{
    public class EvrakSatisEsnafDto
    {
        public List<ComboBoxIdTextDto> TahsilatIslemleri { get; set; }
        public MakbuzDto MakbuzBilgisi { get; set; }
    }
}