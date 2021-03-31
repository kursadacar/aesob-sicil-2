using System.Collections.Generic;
using CTD.Core.Entities;

namespace CTD.Dto.SingleDto
{
    public class MakbuzTekrar
    {
        public MakbuzDokum MakbuzDokum { get; set; }
        public List<MakbuzDetay> MakbuzDetaylari { get; set; }
    }
}