using CTD.Core.Entities;
using CTD.Data.GenericRawSql;
using CTD.Data.GenericRepository;
using CTD.Data.UnitofWork;

namespace CTD.Service.Abstracts
{
    public abstract class AKullaniciService
    {
        internal readonly IGenericRepository<Kullanici> _kullaniciRepository;
        internal readonly IGenericRawSql<object> _objectRawSql;
        internal readonly IUnitofWork _uow;

        internal AKullaniciService(IUnitofWork uow)
        {
            _uow = uow;
            _kullaniciRepository = uow.GetRepository<Kullanici>();
            _objectRawSql = uow.GetRawSql<object>();
        }
    }
}