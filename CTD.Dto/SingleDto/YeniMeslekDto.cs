using System.Collections.Generic;

namespace CTD.Dto.SingleDto
{
    public class YeniMeslekDto
    {
        public List<ComboBoxIdTextDto> Ilces { get; set; }
        public List<ComboBoxIdTextDto> MeslekOdasis { get; set; }
        public List<ComboBoxIdTextDto> Sinifs { get; set; }
    }
}