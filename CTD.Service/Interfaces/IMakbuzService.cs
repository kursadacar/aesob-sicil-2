using CTD.Core.Entities;

namespace CTD.Service.Interfaces
{
    public interface IMakbuzService
    {
        Makbuz GetirKullaniciMakbuz(int kullanici);
        Makbuz GetirKullaniciMakbuz(int kullanici, int id);
        void MakbuzGuncelle(Makbuz model);
    }
}