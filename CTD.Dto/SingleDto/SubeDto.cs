using System;

namespace CTD.Dto.SingleDto
{
    public class SubeDto
    {
        public int Id { get; set; }
        public int SicilId { get; set; }
        public int NaceKodu { get; set; }
        public string IsyeriUnvani { get; set; }
        public int MeslekOdasi { get; set; }
        public int Meslek { get; set; }
        public int IsAdresIlce { get; set; }
        public int IsAdresMahalle { get; set; }
        public int IsAdresCadSokBulv { get; set; }
        public string IsAdres { get; set; }
        public string IsAdres2 { get; set; }
        public int Sinif { get; set; }
        public DateTime KayitTar { get; set; }
        public DateTime SonDegisTar { get; set; }
        public DateTime SonVizeIsTar { get; set; }
        public DateTime VizeSuresiBitTar { get; set; }
        public int KayitEden { get; set; }
        public DateTime KayitEdenTar { get; set; }
        public bool Merkez { get; set; }
        public int MerkezId { get; set; }
    }
}