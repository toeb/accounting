using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public interface IAccountingFacade
  {
    /// <summary>
    /// Checks whether any entries exist in the database.
    /// An empty database is always initialized.
    /// For a database with existing entries, the command settings decide, how to
    /// handle the initialization.
    /// </summary>
    /// <param name="command"></param>
    void InitializeAccounting(InitializeAccountingCommand command);

    /// <summary>
    /// Opens an account / performs validation on the account which is to be opened
    /// throws Invalidoperatioexception if the commands values are not valid
    /// </summary>
    /// <param name="command"></param>
    void OpenAccount(OpenAccountCommand command);

    /// <summary>
    /// Updates an existing account by modifying any of the secondary properties
    /// Number, ShortName, Name
    /// </summary>
    /// <param name="command"></param>
    void UpdateAccount(UpdateAccountCommand command);


    void BillTransaction(BillTransactionCommand command);

    /// <summary>
    /// causes a transaction to be reverted - a new transaction with inverted values is added and connected to the original transaction
    /// </summary>
    /// <param name="command"></param>
    void RevertTransaction(RevertTransactionCommand command);


    void ListAccounts(ListAccountsCommand command);

  }



}
