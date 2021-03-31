using System;

namespace CTD.Dto.ListedDto
{
    public class YazismaReportList
    {
        public int ID { get; set; }
        public DateTime? KAYITTAR { get; set; }
        public DateTime? MESLEKTERKTAR { get; set; }
        public string meslek { get; set; }
    }
}