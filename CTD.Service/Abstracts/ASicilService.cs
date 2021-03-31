using CTD.Core.Entities;
using CTD.Data.GenericRepository;
using CTD.Data.UnitofWork;

namespace CTD.Service.Abstracts
{
    public abstract class ASicilService
    {
        internal readonly IGenericRepository<CadSokBulv> _cadSokBulvRepository;
        internal readonly IGenericRepository<Ilce> _ilceRepository;
        internal readonly IGenericRepository<IslemTurleri> _islemTurleri;
        internal readonly IGenericRepository<Kullanici> _kullaniciRepository;
        internal readonly IGenericRepository<Mahalle> _mahalleRepository;
        internal readonly IGenericRepository<MakbuzDokum> _makbuzDokumRepository;
        internal readonly IGenericRepository<Meslekler> _mesleklerRepository;
        internal readonly IGenericRepository<MeslekOdasi> _meslekOdasiRepository;
        internal readonly IGenericRepository<MeslekTerkNedeni> _meslekTerkNedeniRepository;
        internal readonly IGenericRepository<Nace> _naceRepository;
        internal readonly IGenericRepository<SicilMeslekDegisiklik_Log> _sicilMeslekDegisiklik_LogRepository;
        internal readonly IGenericRepository<SicilMeslek> _sicilMeslekRepository;
        internal readonly IGenericRepository<Sicil> _sicilRepository;
        internal readonly IGenericRepository<Sinif> _sinifRepository;
        internal readonly IGenericRepository<TahsilatIslemleri> _tahsilatIslemleriRepository;
        internal readonly IGenericRepository<TahsilatTurleri> _tahsilatTurleri;
        internal readonly IUnitofWork _uow;

        internal ASicilService(IUnitofWork uow)
        {
            _uow = uow;
            _sicilRepository = uow.GetRepository<Sicil>();
            _sicilMeslekRepository = uow.GetRepository<SicilMeslek>();
            _sicilMeslekDegisiklik_LogRepository = uow.GetRepository<SicilMeslekDegisiklik_Log>();
            _ilceRepository = uow.GetRepository<Ilce>();
            _mahalleRepository = uow.GetRepository<Mahalle>();
            _cadSokBulvRepository = uow.GetRepository<CadSokBulv>();
            _meslekOdasiRepository = uow.GetRepository<MeslekOdasi>();
            _sinifRepository = uow.GetRepository<Sinif>();
            _mesleklerRepository = uow.GetRepository<Meslekler>();
            _naceRepository = uow.GetRepository<Nace>();
            _meslekTerkNedeniRepository = uow.GetRepository<MeslekTerkNedeni>();
            _kullaniciRepository = uow.GetRepository<Kullanici>();
            _tahsilatIslemleriRepository = uow.GetRepository<TahsilatIslemleri>();
            _tahsilatTurleri = uow.GetRepository<TahsilatTurleri>();
            _islemTurleri = uow.GetRepository<IslemTurleri>();
            _makbuzDokumRepository = uow.GetRepository<MakbuzDokum>();
        }
    }
}