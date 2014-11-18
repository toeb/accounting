using Accounting.Model;
using System;
using System.ComponentModel.Composition;
using System.Data.Entity;

namespace Accounting.DataLayer
{
  [Export(typeof(IUnitOfWork))]
  public class UnitOfWork : IUnitOfWork
  {
    private DbContext context;
    private IRepository<Account> accountRepo;

    public UnitOfWork() : this("DefaultConnection") { }

    public UnitOfWork(string nameOrConnectionString)
    {
      context = new AccountingDbContext(nameOrConnectionString);
    }

    [ImportingConstructor]
    public UnitOfWork([Import]DbContext context)
    {
      this.context = context;
    }

    public IRepository<Account> AccountRepository
    {
      get
      {
        if (accountRepo == null) accountRepo = new RepositoryBase<Account>(context);
        return accountRepo;
      }
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
