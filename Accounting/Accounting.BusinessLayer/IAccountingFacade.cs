﻿using System;
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
    /// <param name="command"></param>
    ICommandHandler<OpenAccountCommand> OpenAccountCommandHandler();

    /// <summary>
    /// Updates an existing account by modifying any of the secondary properties
    /// Number, ShortName, Name
    /// </summary>
    /// <param name="command"></param>
    void UpdateAccount(UpdateAccountCommand command);

    /// <summary>
    /// Closes an existing account by setting the isActive field to false
    /// </summary>
    /// <param name="command">Contains the account to close</param>
    void CloseAccount(CloseAccountCommand command);

    void BillTransaction(BillTransactionCommand command);

    /// <summary>
    /// causes a transaction to be reverted - a new transaction with inverted values is added and connected to the original transaction
    /// </summary>
    /// <param name="command"></param>
    void RevertTransaction(RevertTransactionCommand command);


    void ListAccounts(ListAccountsCommand command);

  }



}
