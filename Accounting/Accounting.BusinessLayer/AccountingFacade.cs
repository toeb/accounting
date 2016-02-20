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
    // ... other handler creation methods to follow

    #endregion


    /// <exception cref="System.ArgumentNullException">May be thrown when command is null</exception>
    /// <exception cref="System.ArgumentException">May be thrown when command account id is 0 or less</exception>
    /// <exception cref="System.InvalidOperationException">May be thrown when command properties are invalid</exception>
    public void UpdateAccount(UpdateAccountCommand command)
    {
      if (command == null) throw new ArgumentNullException("command");
      if (command.AccountId <= 0) throw new ArgumentException("command.AccountId");
      if (command.ModifiedAccount != null) throw new ArgumentException("command.ModifiedAccount expected to be null");
      if (command.NewName != null && string.IsNullOrWhiteSpace(command.NewName)) throw new InvalidOperationException("account name may not be whitespace or empty");
      if (command.NewShortName != null && string.IsNullOrWhiteSpace(command.NewShortName)) throw new InvalidOperationException("acccount number may not be whitespace or empty");
      if (command.NewNumber != null && string.IsNullOrWhiteSpace(command.NewNumber)) throw new InvalidOperationException("acccount number may not be whitespace or empty");

      var account = UnitOfWork.GetRepository<Account>().GetByID(command.AccountId);

      if (command.NewName != null)
      {
        if (UnitOfWork.GetRepository<Account>().Get(acc => acc.Name == command.NewName).Any()) throw new InvalidOperationException("account name is not unique");
        account.Name = command.NewName;
      }

      if (command.NewShortName != null)
      {
        if (UnitOfWork.GetRepository<Account>().Get(acc => acc.ShortName == command.NewShortName).Any()) throw new InvalidOperationException("account shortname is not unique");
        account.ShortName = command.NewShortName;
      }

      if (command.NewNumber != null)
      {
        if (UnitOfWork.GetRepository<Account>().Get(acc => acc.Number == command.NewNumber).Any()) throw new InvalidOperationException("account number is not unique");
        account.Number = command.NewNumber;
      }

      UnitOfWork.GetRepository<Account>().Update(account);

      command.ModifiedAccount = account;

      UnitOfWork.Save();
    }


    // checks balance for specified account, requires 0 balance
    private void CheckAccountForClose(Account account)
    {
      var TransRepo = UnitOfWork.GetRepository<PartialTransaction>();
      decimal Credit = TransRepo.Get(x => x.Account.Id == account.Id && x.Type == PartialTransactionType.Credit).Sum(x => x.Amount);
      decimal Debit = TransRepo.Get(x => x.Account.Id == account.Id && x.Type == PartialTransactionType.Debit).Sum(x => x.Amount);

      if (Credit != Debit) throw new InvalidOperationException("Account " + account.Id + " is not balanced!");
    }
    // recursively checks the startAt account and all of its children
    private void CheckAccountsForCloseRecursive(Account startAt)
    {
      if (startAt.IsActive)
      {
        CheckAccountForClose(startAt);
      }

      // FIX: Children list is not correctly loaded if empty
      if (startAt.Children != null)
      {
        foreach (var child in startAt.Children)
        {
          CheckAccountsForCloseRecursive(child);
        }
      }
    }
    // sets isActive of account to false and marks the change for saving
    private void CloseAccountInternal(Account account)
    {
      account.IsActive = false;
      UnitOfWork.GetRepository<Account>().Update(account);
    }
    // recursively closing all children accounts before closing startAt
    private void CloseAccountsRecursive(Account startAt)
    {
      // FIX: Children list is not correctly loaded if empty
      if (startAt.Children != null)
      {
        foreach (var child in startAt.Children)
        {
          CloseAccountsRecursive(child);
        }
      }

      if (startAt.IsActive)
      {
        CloseAccountInternal(startAt);
      }
    }


    public void CloseAccount(CloseAccountCommand command)
    {
      if (command == null) throw new ArgumentNullException("command");
      if (command.AccountId <= 0) throw new ArgumentException("command.AccountId");
      if (command.ClosedAccount != null) throw new ArgumentException("command.ClosedAccount expected to be null");

      var AccToClose = UnitOfWork.GetRepository<Account>().GetByID(command.AccountId);
      if (AccToClose == null) throw new InvalidOperationException("The specified Account does not exist!");
      if (!AccToClose.IsActive) throw new InvalidOperationException("The specified Account is already closed!");

      if (command.Recursive)
      {
        CheckAccountsForCloseRecursive(AccToClose);
        CloseAccountsRecursive(AccToClose);
      }
      else
      {
        CheckAccountForClose(AccToClose);
        // FIX: empty children-list may be null instead
        if (AccToClose.Children != null && AccToClose.Children.Any(x => x.IsActive)) throw new InvalidOperationException("Account " + AccToClose.Id + " has active children and the close command was not called recursively!");
        CloseAccountInternal(AccToClose);
      }
      command.ClosedAccount = AccToClose;

      UnitOfWork.Save();
    }


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
