using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer.QueryHandlers
{
  // future: user authorization
  class ListAccountsQueryHandler : ICommandHandler<ListAccountsCommand>
  {
    private IUnitOfWork UnitOfWork;

    public ListAccountsQueryHandler(IUnitOfWork UnitOfWork)
    {
      this.UnitOfWork = UnitOfWork;
    }

    public void Handle(ListAccountsCommand command)
    {
      var accounts = UnitOfWork.GetRepository<Account>();
      command.Query = accounts.Read().Where(acc => acc.IsActive);
    }

    public bool Validate(ListAccountsCommand command, out IEnumerable<Exception> ValidationErrors)
    {
      ValidationErrors = null;
      return true;
    }
  }
}
