using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class SicilSingleDto
    {
        public Sicil Sicil { get; set; }
        public SicilMeslek SicilMeslek { get; set; }
        public int? NaceId { get; set; }
        public int? MeslekId { get; set; }
        public string Meslek { get; set; }
        public string NaceTanimi { get; set; }
        public string Adres { get; set; }
    }
}