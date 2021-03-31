using System;
using CTD.Data.GenericRawSql;
using CTD.Data.GenericRepository;

namespace CTD.Data.UnitofWork
{
    public interface IUnitofWork : IDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        IGenericRawSql<TEntity> GetRawSql<TEntity>() where TEntity : class;
        int SaveChanges();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}