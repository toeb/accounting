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
    // ... other handler creation methods to follow

    #endregion



    private static IEnumerable<PartialTransaction> CreatePartialTransactions(
      IRepository<Account> Accounts,
      IEnumerable<AddPartialTransactionCommand> partials,
      PartialTransactionType type)
    {

      // check if accounts exist
      foreach (var partial in partials)
      {
        var obj = new PartialTransaction()
        {
          Type = type,
          Amount = partial.Amount,
          Account = Accounts.GetByID(partial.AccountId)
        };
        if (obj.Account == null) throw new InvalidOperationException("partial transaction needs to have an existing account");
        if (!obj.Account.IsActive) throw new InvalidOperationException("transaction may touch only active accounts");

        yield return obj;
      }
    }

    public void BillTransaction(BillTransactionCommand command)
    {
      // get repository
      var Accounts = UnitOfWork.GetRepository<Account>();
      var Transactions = UnitOfWork.GetRepository<Transaction>();

      Trace.TraceInformation("Billing a transaction");


      if (command == null) throw new ArgumentNullException("command");

      if (string.IsNullOrWhiteSpace(command.TransactionText)) throw new InvalidOperationException("transaction does not have a text");
      if (string.IsNullOrWhiteSpace(command.Receipt)) throw new InvalidOperationException("transaction does not have a receipt");
      if (!command.ReceiptDate.HasValue) throw new InvalidOperationException("transaction needs a valid receipt date");


      if (command.Credits == null) throw new InvalidOperationException("credits may not be null");
      if (command.Debits == null) throw new InvalidOperationException("debits may not be null");
      if (command.Credits.Any(p => p.Amount <= 0.0m)) throw new InvalidOperationException("partial transactions may not have an amount of 0 or less");
      if (command.Debits.Any(p => p.Amount <= 0.0m)) throw new InvalidOperationException("partial transactions may not have an amount of 0 or less");

      // check if balanced

      var creditSum = command.Credits.Aggregate(0.0m, (lhs, p) => lhs + p.Amount);
      var debitSum = command.Debits.Aggregate(0.0m, (lhs, p) => lhs + p.Amount);

      if (creditSum != debitSum) throw new InvalidOperationException("transaction is not balanced");


      var transactions = CreatePartialTransactions(Accounts, command.Credits, PartialTransactionType.Credit)
        .Concat(CreatePartialTransactions(Accounts, command.Debits, PartialTransactionType.Debit)).ToList();




      Trace.TraceInformation("Transaction is valid adding it to database");

      var transaction = new Transaction()
      {
        ReceiptDate = command.ReceiptDate.Value,
        ReceiptNumber = command.Receipt,
        Text = command.TransactionText,
        Storno = null,
        CreationDate = DateTime.Now,
        LastModified = DateTime.Now,
        Partials = transactions

      };

      Transactions.Create(transaction);
      UnitOfWork.Save();

      Trace.TraceInformation("Transaction was added to database");
      command.Transaction = transaction;
    }




    public void RevertTransaction(RevertTransactionCommand command)
    {

      var transactions = UnitOfWork.GetRepository<Transaction>();
      var accounts = UnitOfWork.GetRepository<Account>();


      var transactionToRevert = transactions.GetByID(command.TransactionId);
      if (transactionToRevert == null) throw new InvalidOperationException("the transaction to revert does not exist");


      var revertedTransaction = new Transaction()
      {
        Storno = transactionToRevert,
        ReceiptDate = transactionToRevert.ReceiptDate,
        ReceiptNumber = transactionToRevert.ReceiptNumber,
        Text = string.IsNullOrWhiteSpace(command.Text) ? "Storno: " + transactionToRevert.Text : command.Text,
        Partials = transactionToRevert.Partials.Select(p => new PartialTransaction() { Amount = -p.Amount, Type = p.Type }).ToList()
      };

      transactionToRevert.Storno = revertedTransaction;

      transactions.Create(revertedTransaction);
      transactions.Update(transactionToRevert);
      UnitOfWork.Save();

      transactions.Refresh(revertedTransaction.Storno);
      transactions.Refresh(revertedTransaction);

      command.RevertedTransaction = revertedTransaction;
    }



    public void ListAccounts(ListAccountsCommand command)
    {
      /// future: chck if user is allowed to access account
      var accounts = UnitOfWork.GetRepository<Account>();
      command.Query = accounts.Read().Where(acc => acc.IsActive);
    }
  }
}
