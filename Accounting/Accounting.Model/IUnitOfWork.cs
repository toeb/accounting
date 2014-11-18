using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Model
{
  public interface IUnitOfWork : IDisposable
  {
    IRepository<Account> AccountRepository { get; }

    void Save();
  }

}
