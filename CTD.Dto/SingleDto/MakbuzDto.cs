using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class MakbuzDto
    {
        public int? Id { get; set; }
        public string OnTaki { get; set; }
        public int? MakbuzNo { get; set; }
        public int? KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; }
    }
}