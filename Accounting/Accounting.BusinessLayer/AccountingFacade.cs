using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;

namespace Accounting.BusinessLayer
{

  [Export(typeof(IAccountingFacade))]
  public class AccountingFacade : IAccountingFacade
  {
    [Import]
    public IUnitOfWork UnitOfWork { get; set; }


    public void InitializeAccounting(InitializeAccountingCommand command)
    {
      var accountRepo = UnitOfWork.GetRepository<Account>();

      // check for preexisting data
      if (accountRepo.Read().Any())
      {
        switch (command.InitializationAction)
        {
          case InitializationActions.ExpectNewDatabase:
            throw new InvalidOperationException("Expected a clean database!");
          case InitializationActions.DropInactive:
            if (accountRepo.Read().Any(x => x.IsActive)) throw new InvalidOperationException("Expected only inactive accounts!");
            throw new NotImplementedException();
          case InitializationActions.DropOld:
            throw new NotImplementedException();
          case InitializationActions.RepairOld:
            throw new NotImplementedException();
        }
      }

      var categories = UnitOfWork.GetRepository<AccountCategory>().Read();
      var categoryCategory = categories.First(x => x.Name == "Kategorie");

      var catMoney = new Account() { Number = "C01", Name = "Konten", AccountCategory = categoryCategory, ShortName = "Konten" };
      accountRepo.Create(catMoney);

      var catBalance = new Account() { Number = "C02", Name = "Sachkonten", AccountCategory = categoryCategory, ShortName = "Konten" };
      accountRepo.Create(catBalance);

      var catCustomers = new Account() { Number = "C03", Name = "Personenkonten", AccountCategory = categoryCategory, ShortName = "Konten" };
      accountRepo.Create(catCustomers);

      var accountCategory = categories.First(x => x.Name == "Konto");

      var accBankAccount = new Account() { Number = "K000001", Name = "Konto der Aktivitas", AccountCategory = accountCategory, ShortName = "Konto", Parent = catMoney };
      accountRepo.Create(accBankAccount);

      var accEquity = new Account() { Number = "S000001", Name = "Eigenkapital", AccountCategory = accountCategory, ShortName = "Eigenkapital", Parent = catBalance };
      accountRepo.Create(accEquity);

      var accSample = new Account() { Number = "P000001", Name = "Beispielkonto für Personen", AccountCategory = accountCategory, ShortName = "Beispielkonto", Parent = catCustomers };
      accountRepo.Create(accSample);

      UnitOfWork.Save();
    }


    public void OpenAccount(OpenAccountCommand command)
    {
      if (command == null) throw new ArgumentNullException("command");
      if (string.IsNullOrWhiteSpace(command.AccountName)) throw new InvalidOperationException("accountname may not be whitespace empty");
      if (string.IsNullOrWhiteSpace(command.AccountNumber)) throw new InvalidOperationException("acccount number may not be null or empty");

      if (UnitOfWork.GetRepository<Account>().Get(acc => acc.Name == command.AccountName || acc.Number == command.AccountNumber).Any()) throw new InvalidOperationException("account name or number is not unique");

      Account parent = null;
      if (command.ParentAccountId.HasValue)
      {
        parent = UnitOfWork.GetRepository<Account>().GetByID(command.ParentAccountId);
      }


      var account = new Account()
      {
        Name = command.AccountName,
        Number = command.AccountNumber,
        Parent = parent,
        IsActive = true
      };

      UnitOfWork.GetRepository<Account>().Create(account);

      command.Account = account;

      UnitOfWork.Save();
    }

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
      
      if(string.IsNullOrWhiteSpace(command.TransactionText))throw new InvalidOperationException("transaction does not have a text");
      if(string.IsNullOrWhiteSpace(command.Receipt))throw new InvalidOperationException("transaction does not have a receipt");
      if(!command.ReceiptDate.HasValue)throw new InvalidOperationException("transaction needs a valid receipt date");


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

      var transaction= new Transaction(){
        ReceiptDate = command.ReceiptDate.Value,
        ReceiptNumber = command.Receipt,
        Text = command.TransactionText,
        Storno= null,
        CreationDate = DateTime.Now,
        LastModified= DateTime.Now,
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


      var transactionToRevert= transactions.GetByID(command.TransactionId);
      if (transactionToRevert == null) throw new InvalidOperationException("the transaction to revert does not exist");


      var revertedTransaction = new Transaction()
      {
        Storno = transactionToRevert,
        ReceiptDate = transactionToRevert.ReceiptDate,
        ReceiptNumber = transactionToRevert.ReceiptNumber,
        Text = string.IsNullOrWhiteSpace(command.Text)?"Storno: "+ transactionToRevert.Text:command.Text,
        Partials = transactionToRevert.Partials.Select(p => new PartialTransaction() { Amount = -p.Amount, Type = p.Type}).ToList()
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
