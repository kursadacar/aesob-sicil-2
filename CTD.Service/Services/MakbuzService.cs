using System.Linq;
using CTD.Core.Entities;
using CTD.Data.GenericRepository;
using CTD.Data.UnitofWork;
using CTD.Service.Interfaces;

namespace CTD.Service.Services
{
    public class MakbuzService : IMakbuzService
    {
        private readonly IGenericRepository<Makbuz> _makbuzRepository;

        public MakbuzService(IUnitofWork uow)
        {
            _makbuzRepository = uow.GetRepository<Makbuz>();
        }

        public Makbuz GetirKullaniciMakbuz(int kullanici)
        {
            return _makbuzRepository.GetAll().FirstOrDefault(makbuz => makbuz.KULLANICI == kullanici);
        }

        public Makbuz GetirKullaniciMakbuz(int kullanici, int id)
        {
            return _makbuzRepository.GetAll()
                .FirstOrDefault(makbuz => makbuz.KULLANICI == kullanici && makbuz.Id == id);
        }

        public void MakbuzGuncelle(Makbuz model)
        {
            _makbuzRepository.Update(model);
        }
    }
}