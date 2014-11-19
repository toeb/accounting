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
    [Import]
    public IRepository<Account> Accounts { get; set; }

    [Import]
    public IRepository<Transaction> Transactions { get; set; }

    public void OpenAccount(OpenAccountCommand command)
    {
      if (command == null) throw new ArgumentNullException("command");
      if (string.IsNullOrWhiteSpace(command.AccountName)) throw new InvalidOperationException("accountname may not be whitespace empty");
      if (string.IsNullOrWhiteSpace(command.AccountNumber)) throw new InvalidOperationException("acccount number may not be null or empty");

      if (Accounts.Get(acc => acc.Name == command.AccountName || acc.Number == command.AccountNumber).Any()) throw new InvalidOperationException("account name or number is not unique");

      Account parent = null;
      if (command.ParentAccountId.HasValue)
      {
        parent = Accounts.GetByID(command.ParentAccountId);
      }


      var account = new Account()
      {
        Name = command.AccountName,
        Number = command.AccountNumber,
        Parent = parent,
        IsActive = true
      };

      Accounts.Insert(account);

      command.Account = account;


    }



    public void BillTransaction(BillTransactionCommand command)
    {
      Trace.TraceInformation("Billing a transaction");

      
      if (command == null) throw new ArgumentNullException("command");
      
      if(string.IsNullOrWhiteSpace(command.TransactionText))throw new InvalidOperationException("transaction does not have a text");
      if(string.IsNullOrWhiteSpace(command.Receipt))throw new InvalidOperationException("transaction does not have a receipt");
      if(!command.ReceiptDate.HasValue)throw new InvalidOperationException("transaction needs a valid receipt date");

      if (command.PartialTransactions == null) throw new InvalidOperationException("partial transactions may not be null");
      if (command.PartialTransactions.Count() < 2) throw new InvalidOperationException("at least two partial transactions are necessary to create a transaction");
      if (!command.PartialTransactions.Any(p => p.Type == PartialTransactionType.Credit)) throw new InvalidOperationException("at least one partial transaction needs to be credit");
      if (!command.PartialTransactions.Any(p => p.Type == PartialTransactionType.Debit)) throw new InvalidOperationException("at least one partial transaction needs to be debit");
      if (command.PartialTransactions.Any(p => p.Amount <= 0.0m)) throw new InvalidOperationException("partial transactions may not have an amount of 0 or less");

      
      // check if accounts exist
      foreach (var partial in command.PartialTransactions)
      {
        if (partial.Account == null) throw new InvalidOperationException("partial transaction needs to have an account");
        partial.Account = Accounts.GetByID(partial.Account.Id);
        if (partial.Account == null) throw new InvalidOperationException("partial transaction needs to have an existing account");
        if (!partial.Account.IsActive) throw new InvalidOperationException("transaction may touch only active accounts");
      }



      // check if balanced
      var debits = command.PartialTransactions.Where(p => p.Type == PartialTransactionType.Debit);
      var credits = command.PartialTransactions.Where(p => p.Type == PartialTransactionType.Credit);

      var creditSum = credits.Aggregate(0.0m, (lhs, p) => lhs + p.Amount);
      var debitSum = credits.Aggregate(0.0m, (lhs, p) => lhs + p.Amount);

      if (creditSum != debitSum) throw new InvalidOperationException("transaction is not balanced");


      Trace.TraceInformation("Transaction is valid adding it to database");

      var transaction= new Transaction(){
        ReceiptDate = command.ReceiptDate.Value,
        ReceiptNumber = command.Receipt,
        Text = command.TransactionText,
        Storno= null,
        CreationDate = DateTime.Now,
        LastModified= DateTime.Now
        
      };
      transaction.Partials = command.PartialTransactions.ToList();

      Transactions.Insert(transaction);
      // IUnitOfWork.Save()

      Trace.TraceInformation("Transaction was added to database");
      command.Transaction = transaction;
    }




    public void RevertTransaction(RevertTransactionCommand command)
    {
      throw new NotImplementedException();
    }

  }
}
