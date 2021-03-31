using System.Collections.Generic;
using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class MakbuzTahsilatModelDto
    {
        public virtual MakbuzDto Makbuz { get; set; }
        public List<TahsilatKalemleri> TahsilatKalemleri { get; set; }
        public virtual IList<SicilMeslek> SicilMeslekleri { get; set; }
        public List<ComboBoxIdTextDto> UyeMeslekOdaList { get; set; }
        public string AdSoyad { get; set; }
        public int? KullaniciId { get; set; }
        public int? MeslekOdasiId { get; set; }
        public List<ComboBoxIdTextDto> Kullanicilar { get; set; }
    }
}