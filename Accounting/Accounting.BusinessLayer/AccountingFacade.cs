using Accounting.BusinessLayer.CommandHandlers;
using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{

  [Export(typeof(IAccountingFacade))]
  public class AccountingFacade : IAccountingFacade
  {
    protected IUnitOfWork UnitOfWork;

    [ImportingConstructor]
    public AccountingFacade(IUnitOfWork unitOfWork)
    {
      this.UnitOfWork = unitOfWork;
    }

    #region Get Handler methods

    /**************************************************************
     * Decorator concept:
     * - Define common actions that should be executed before/after
     *   Validate(...) or Handle(...) of a specific handler.
     * 
     * Example: ensure, that Validate(...) is always called before
     *   Handle(...)
     **************************************************************/
    /// <summary>
    /// Decorate a handler with common pre/post tasks
    /// </summary>
    /// <returns>A compatible ICommandHandler object, that probably contains the original handler</returns>
    protected ICommandHandler<TCommand> DecorateHandler<TCommand>(ICommandHandler<TCommand> handler)
    {
      return new ValidatingHandlerDecorator<TCommand>(handler);
    }

    //------------------------------------------------------------------------

    public ICommandHandler<OpenAccountCommand> OpenAccountCommandHandler()
    {
      return DecorateHandler(new OpenAccountCommandHandler(UnitOfWork));
    }

    public ICommandHandler<UpdateAccountCommand> UpdateAccountCommandHandler()
    {
      return DecorateHandler(new UpdateAccountCommandHandler(UnitOfWork));
    }

    public ICommandHandler<CloseAccountCommand> CloseAccountCommandHandler()
    {
      return DecorateHandler(new CloseAccountCommandHandler(UnitOfWork));
    }

    public ICommandHandler<BillTransactionCommand> BillTransactionCommandHandler()
    {
      return DecorateHandler(new BillTransactionCommandHandler(UnitOfWork));
    }

    public ICommandHandler<RevertTransactionCommand> RevertTransactionCommandHandler()
    {
      return DecorateHandler(new RevertTransactionCommandHandler(UnitOfWork));
    }
    // ... other handler creation methods to follow

    #endregion



    public void ListAccounts(ListAccountsCommand command)
    {
      /// future: chck if user is allowed to access account
      var accounts = UnitOfWork.GetRepository<Account>();
      command.Query = accounts.Read().Where(acc => acc.IsActive);
    }
  }
}
