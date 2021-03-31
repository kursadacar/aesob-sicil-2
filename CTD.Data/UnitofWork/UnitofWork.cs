﻿using CTD.Data.Context; using CTD.Data.GenericRawSql; using CTD.Data.GenericRepository; using System; using System.Data.Entity;  namespace CTD.Data.UnitofWork { public class UnitofWork : IUnitofWork { private readonly DatabaseContext _context; private readonly CommonContext _cContext; private bool disposed = false; private DbContextTransaction trans = null; public UnitofWork(DatabaseContext context, CommonContext cContext) { Database.SetInitializer<DatabaseContext>(null); if (context == null) throw new ArgumentNullException("context is null"); _context = context; _cContext = cContext; } public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class { return new GenericRepository<TEntity>(_context); } public IGenericRawSql<TEntity> GetRawSql<TEntity>() where TEntity : class { return new GenericRawSql<TEntity>(_cContext); }  public int SaveChanges() { return _context.SaveChanges(); }  public void BeginTransaction() { trans = _context.Database.BeginTransaction(); }  public void Commit() { trans.Commit(); }  public void Rollback() { trans.Rollback(); }  public virtual void Dispose(bool disposing) { if (!this.disposed) { if (disposing) { _context.Dispose(); } } this.disposed = true; } public void Dispose() { Dispose(true); GC.SuppressFinalize(this); } } } 