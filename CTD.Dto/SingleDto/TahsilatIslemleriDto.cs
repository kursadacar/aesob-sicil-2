using System;

namespace CTD.Dto.SingleDto
{
    public class TahsilatIslemleriDto
    {
        public int? Id { get; set; }
        public int? SicilMeslekId { get; set; }
        public int? SicilId { get; set; }
        public int? IslemTuruId { get; set; }
        public int? UserId { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public string Meslek { get; set; }
        public string IslemTuru { get; set; }
        public string User { get; set; }
    }
}