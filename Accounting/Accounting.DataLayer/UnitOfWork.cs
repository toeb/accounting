using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;

namespace Accounting.DataLayer
{
  [Export(typeof(IUnitOfWork))]
  public class UnitOfWork : IUnitOfWork
  {
    private DbContext context;
    private Dictionary<Type, object> repo = new Dictionary<Type, object>();

    public UnitOfWork() : this("DefaultConnection") { }

    public UnitOfWork(string nameOrConnectionString)
    {
      this.context = new AccountingDbContext(nameOrConnectionString);
    }

    [ImportingConstructor]
    public UnitOfWork([Import]DbContext context)
    {
      this.context = context;
    }

    public IRepository<T> GetRepository<T>() where T : Meta
    {
        Type t = typeof(T);
        if (!repo.ContainsKey(t))
        {
            repo.Add(t, new RepositoryBase<T>(context));
        }
        return (IRepository<T>) repo[t];
    }

    public void Save()
    {
      context.SaveChanges();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          context.Dispose();
        }
      }
      this.disposed = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

  }
}
