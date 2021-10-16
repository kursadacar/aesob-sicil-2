using System.Collections.Generic;
using System.Linq;
using CTD.Core.Entities;
using CTD.Data.GenericRawSql;
using CTD.Data.GenericRepository;
using CTD.Data.UnitofWork;
using CTD.Dto.SingleDto;
using CTD.Service.Interfaces;

namespace CTD.Service.Services
{
    public class TanimService : ITanimService
    {
        private readonly IGenericRepository<CadSokBulv> _cadsokbulvRepository;
        private readonly IGenericRepository<Ilce> _ilceRepository;
        private readonly IGenericRepository<Mahalle> _mahalleRepository;
        private readonly IGenericRepository<Meslekler> _mesleklerRepository;
        private readonly IGenericRepository<MeslekOdasi> _meslekOdasiRepository;
        private readonly IGenericRepository<MeslekTerkNedeni> _meslekTerkNedeniRepository;
        private readonly IGenericRepository<Nace> _naceRepository;
        internal readonly IGenericRawSql<object> _objectRawSql;

        public TanimService(IUnitofWork uow)
        {
            _mesleklerRepository = uow.GetRepository<Meslekler>();
            _naceRepository = uow.GetRepository<Nace>();
            _objectRawSql = uow.GetRawSql<object>();
            _ilceRepository = uow.GetRepository<Ilce>();
            _mahalleRepository = uow.GetRepository<Mahalle>();
            _cadsokbulvRepository = uow.GetRepository<CadSokBulv>();
            _meslekOdasiRepository = uow.GetRepository<MeslekOdasi>();
            _meslekTerkNedeniRepository = uow.GetRepository<MeslekTerkNedeni>();
        }

        public void EkleNace(Nace entity)
        {
            _naceRepository.Insert(entity);
        }

        public List<Ilce> TumIlceler()
        {
            return _ilceRepository.GetAll().OrderBy(s => s.ILCE).ToList();
        }

        public List<Mahalle> IlceyeGoreMahalleler(int? ilceid)
        {
            return _mahalleRepository.GetAll().Where(s => s.ILCEID == ilceid).ToList();
        }
        
        public bool MahalleDuzenle(Mahalle mahalle)
        {
            var mah = _mahalleRepository.GetAll().FirstOrDefault(s => s.Id == mahalle.Id);
            if(mahalle != null)
            {
                mah.MAHALLE = mahalle.MAHALLE;
                _mahalleRepository.Update(mah);
                return true;
            }
            return false;
        } 

        public bool MahalleSil(Mahalle mahalle)
        {
            var mah = _mahalleRepository.GetAll().FirstOrDefault(s => s.Id == mahalle.Id);
            if(mahalle != null)
            {
                mah.MAHALLE = mahalle.MAHALLE;
                _mahalleRepository.Delete(mah);
                return true;
            }
            return false;
        }

        public List<CadSokBulv> MahalleyeGoreCadSoklar(int? mahalleid)
        {
            return _cadsokbulvRepository.GetAll().Where(s => s.MAHALLEID == mahalleid).ToList();
        }

        public List<MeslekOdasi> GetirOdalar()
        {
            return _meslekOdasiRepository.GetAll().ToList();
        }

        public MeslekOdasi GetirOda(int id)
        {
            return _meslekOdasiRepository.GetAll().FirstOrDefault(c => c.Id == id);
        }

        public void MeslekOdasiGuncelle(MeslekOdasi model)
        {
            _meslekOdasiRepository.Update(model);
        }

        public void MeslekOdasiSil(MeslekOdasi model)
        {
            _meslekOdasiRepository.Delete(model);
        }

        public List<MeslekTerkNedeni> GetirTerkNedenleri()
        {
            return _meslekTerkNedeniRepository.GetAll().ToList();
        }

        public MeslekTerkNedeni GetirTerkNedeni(int id)
        {
            return _meslekTerkNedeniRepository.GetAll().FirstOrDefault(c => c.Id == id);
        }

        public void TerkNedeniSil(MeslekTerkNedeni model)
        {
            _meslekTerkNedeniRepository.Delete(model);
        }

        public void TerkNedenikaydet(MeslekTerkNedeni model)
        {
            if (model.Id == 0)
                _meslekTerkNedeniRepository.Insert(model);
            else
                _meslekTerkNedeniRepository.Update(model);
        }

        public List<ComboBoxIdTextDto> ComboBoxTumIlceler()
        {
            var result = (from c in _ilceRepository.GetAll() select c).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.ILCE}).ToList();
            return result;
        }

        public void KaydetMahalle(Mahalle model)
        {
            _mahalleRepository.Insert(model);
        }

        public List<ComboBoxIdTextDto> ComboBoxIlceyeGoreMahalleler(int ilceid)
        {
            var result = (from c in _mahalleRepository.GetAll() where c.ILCEID == ilceid select c).ToList()
                .Select(p => new ComboBoxIdTextDto {id = p.Id.ToString(), text = p.MAHALLE}).ToList();
            return result;
        }

        public void KaydetSokak(CadSokBulv model)
        {
            _cadsokbulvRepository.Insert(model);
        }

        public bool CadSokBulvDuzenle(CadSokBulv model)
        {
            try
            {
                _cadsokbulvRepository.Update(model);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CadSokBulvSil(CadSokBulv model)
        {
            try
            {
                _cadsokbulvRepository.Delete(model);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Meslekler> TumMeslekler()
        {
            return _mesleklerRepository.GetAll().OrderByDescending(s => s.Id).ToList();
        }

        public List<Nace> GetirNaceler(int? meslekid)
        {
            return _naceRepository.GetAll().Where(s => s.MESLEKID == meslekid).ToList();
        }

        public Meslekler GetirMeslek(int meslekid)
        {
            return _mesleklerRepository.Find(meslekid);
        }

        public void MeslekSil(Meslekler meslek)
        {
            _mesleklerRepository.Delete(meslek);
        }
        public void MeslekDuzenle(Meslekler meslek)
        {
            _mesleklerRepository.Update(meslek);
        }

        public Nace GetirNace(int naceid)
        {
            return _naceRepository.Find(naceid);
        }

        public void NaceSil(Nace nace)
        {
            _naceRepository.Delete(nace);
        }
        public void NaceDuzenle(Nace nace)
        {
            _naceRepository.Update(nace);
        }

        public void KaydetMeslek(Meslekler model)
        {
            _mesleklerRepository.Insert(model);
        }

        public void KaydetNace(Nace model)
        {
            _naceRepository.Insert(model);
        }

        public List<ComboBoxIdTextDto> GetirMeslekler()
        {
            return _mesleklerRepository.GetAll().OrderByDescending(p => p.Id).ToList()
                .Select(p => new ComboBoxIdTextDto {text = p.MESLEK, id = p.Id.ToString()}).ToList();
        }
    }
}