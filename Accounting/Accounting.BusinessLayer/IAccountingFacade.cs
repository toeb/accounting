using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public interface IAccountingFacade
  {
    /// <summary>
    /// The returned handler opens an account / performs validation on the account which is to be opened
    /// throws InvalidOperationException if the commands values are not valid.
    /// For multiple invalid properties, the exception contains an inner AggregateException with details
    /// </summary>
    ICommandHandler<OpenAccountCommand> OpenAccountCommandHandler();

    /// <summary>
    /// The returned handler updates an existing account by modifying any of the secondary properties
    /// Number, ShortName, Name
    /// </summary>
    ICommandHandler<UpdateAccountCommand> UpdateAccountCommandHandler();

    /// <summary>
    /// The returned handler closes an existing account by setting the isActive field to false
    /// </summary>
    ICommandHandler<CloseAccountCommand> CloseAccountCommandHandler();

    /// <summary>
    /// The returned handler creates a transaction with all associated partial transactions
    /// </summary>
    ICommandHandler<BillTransactionCommand> BillTransactionCommandHandler();

    /// <summary>
    /// The returned handler causes a transaction to be reverted - a new transaction with inverted values is added and connected to the original transaction
    /// </summary>
    ICommandHandler<RevertTransactionCommand> RevertTransactionCommandHandler();


    void ListAccounts(ListAccountsCommand command);

  }
}
