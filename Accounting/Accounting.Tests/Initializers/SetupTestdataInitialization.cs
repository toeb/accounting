using Accounting.BusinessLayer;
using Accounting.DataLayer;
using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Tests.Initializers
{
  class SetupTestdataInitialization
  {
    private IAccountingFacade Facade { get; set; }
    private AccountingDbContext Context { get; set; }

    public SetupTestdataInitialization(IAccountingFacade facade, AccountingDbContext context)
    {
      Facade = facade;
      Context = context;
    }


    /// <summary>
    /// Add Category of specified name only if not yet present
    /// </summary>
    /// <param name="name">Name of the category to create</param>
    public void SetupCategory(string name)
    {
      if (!Context.AccountCategories.Any(x => x.Name == name))
      {
        Context.AccountCategories.Add(new AccountCategory() { Name = name });
        Context.SaveChanges();
      }
    }


    public void SetupAccount(string name, string number, string shortname, string categoryname, string parentname = null)
    {
      if (!Context.Accounts.Any(x => x.Name == name))
      {
        if (Context.Accounts.Any(x => x.Number == number)) throw new Exception("Bad request! " + number);
        if (Context.Accounts.Any(x => x.ShortName == shortname)) throw new Exception("Bad request! " + shortname);

        SetupCategory(categoryname);
        var parent = Context.Accounts.FirstOrDefault(x => x.Name == parentname);
        Facade.OpenAccountCommandHandler().Handle(new OpenAccountCommand()
        {
          AccountName = name,
          AccountNumber = number,
          CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == categoryname).Id,
          ParentAccountId = (parent == null ? 0 : parent.Id)
        });
      }
    }
    /// <summary>
    /// <para>Initializes the database with basic hierarchie entries:</para>
    /// <para>|Wertkonten|</para>
    /// <para>|Sachkonten|</para>
    /// <para>|Personenkonten|</para>
    /// </summary>
    public void SetupCategoryAccounts()
    {
      SetupAccount("Wertkonten", "#00001", "Wertkonten", "Hauptkategorie");
      SetupAccount("Sachkonten", "#00002", "Sachkonten", "Hauptkategorie");
      SetupAccount("Personenkonten", "#00003", "Personenkonten", "Hauptkategorie");
    }

    private BillTransactionCommand PrepareBillTransactionCommand(string text, string receiptNumber, DateTime receiptDate)
    {
      if (Context.Transactions.Any(x => x.ReceiptNumber == receiptNumber && x.ReceiptDate.Year == receiptDate.Year)) throw new Exception("Non-unique receipt!");

      return new BillTransactionCommand()
      {
        TransactionText = text,
        Receipt = receiptNumber,
        ReceiptDate = receiptDate.Date,
        Credits = new List<AddPartialTransactionCommand>(),
        Debits = new List<AddPartialTransactionCommand>()
      };
    }

    private void AddCreditorToTransaction(BillTransactionCommand trans, string accountName, decimal amount)
    {
      trans.AddCreditor(amount, Context.Accounts.FirstOrDefault(x => x.Name == accountName).Id);
    }

    private void AddDebitorToTransaction(BillTransactionCommand trans, string accountName, decimal amount)
    {
      trans.AddDebitor(amount, Context.Accounts.FirstOrDefault(x => x.Name == accountName).Id);
    }

    /// <summary>
    /// <para>Initializes the database with some entries for more advanced tests. Available accounts in hierarchie:</para>
    /// <para>|Wertkonten| -> |Konto1|</para>
    /// <para>|Sachkonten| -> |Sachkonto1|</para>
    /// <para>|Sachkonten| -> |Sachkonto2|</para>
    /// <para>|Sachkonten| -> |Sachkonto3|</para>
    /// <para>|Personenkonten| -> |Personenkonto1|</para>
    /// <para>|Personenkonten| -> |Personenkonto2|</para>
    /// <para>|Personenkonten| -> |Personenkonto2| -> |Personenkonto2.1|</para>
    /// <para>|Personenkonten| -> |Personenkonto2| -> |Personenkonto2.2|</para>
    /// <para>|Personenkonten| -> |Personenkonto3|</para>
    /// <para>|Personenkonten| -> |Personenkonto3| -> |Personenkonto3.1|</para>
    /// <para>|Personenkonten| -> |Personenkonto3| -> |Personenkonto3.2|</para>
    /// <para>|Personenkonten| -> |Personenkonto4|</para>
    /// <para>|Personenkonten| -> |Personenkonto4| -> |Personenkonto4.1|</para>
    /// <para>|Personenkonten| -> |Personenkonto4| -> |Personenkonto4.2|</para>
    /// <para>Account Activity and Balance for each account:</para>
    /// <para>|konto| = credit: 0 | debit: 30.5 | balance: -30.5</para>
    /// <para>|Sachkonto1| = credit: 5.5 | debit: 0 | balance: 5.5</para>
    /// <para>|Sachkonto2| = credit: 0 | debit: 0 | balance: 0</para>
    /// <para>|Personenkonto1| = credit: 25 | debit: 10 | balance: 15</para>
    /// <para>|Personenkonto2| = credit: 15 | debit: 10 | balance: 5</para>
    /// <para>|Personenkonto2.1| = credit: 15 | debit: 0 | balance: 15</para>
    /// <para>|Personenkonto2.2| = credit: 0 | debit: 10 | balance: -10</para>
    /// <para>|Personenkonto3| = credit: 10 | debit: 10 | balance: 0</para>
    /// <para>|Personenkonto3.1| = credit: 10 | debit: 5 | balance: 5</para>
    /// <para>|Personenkonto3.2| = credit: 0 | debit: 5 | balance: -5</para>
    /// <para>|Personenkonto4| = credit: 5 | debit: 5 | balance: 0</para>
    /// <para>|Personenkonto4.1| = credit: 5 | debit: 5 | balance: 0</para>
    /// <para>|Personenkonto4.2| = credit: 5 | debit: 5 | balance: 0</para>
    /// </summary>
    public void SetupAccountingEnvironment()
    {
      SetupCategoryAccounts();

      SetupAccount("Konto1", "K00001", "KT1", "Buchungskonto", "Wertkonten");

      SetupAccount("Sachkonto1", "S00001", "SK1", "Buchungskonto", "Sachkonten");
      SetupAccount("Sachkonto2", "S00002", "SK2", "Buchungskonto", "Sachkonten");

      SetupAccount("Personenkonto1", "P00001", "PK1", "Buchungskonto", "Personenkonten");
      SetupAccount("Personenkonto2", "P00002", "PK2", "Buchungskonto", "Personenkonten");

      SetupAccount("Personenkonto2.1", "P00002-1", "PK2.1", "Buchungskonto", "Personenkonto2");
      SetupAccount("Personenkonto2.2", "P00002-2", "PK2.2", "Buchungskonto", "Personenkonto2");

      var transaction = PrepareBillTransactionCommand("Test - Zahlungseingang mit Split", "1", new DateTime(1999, 1, 1));

      AddCreditorToTransaction(transaction, "Sachkonto1", 5.5m);
      AddCreditorToTransaction(transaction, "Personenkonto2", 10m);
      AddCreditorToTransaction(transaction, "Personenkonto2.1", 15m);
      AddDebitorToTransaction(transaction, "Konto1", 30.5m);
      Facade.BillTransaction(transaction);

      transaction = PrepareBillTransactionCommand("Test - Many to Many transaction", "2", new DateTime(1999, 1, 1));

      AddCreditorToTransaction(transaction, "Personenkonto1", 25m);
      AddCreditorToTransaction(transaction, "Personenkonto2", 5m);
      AddDebitorToTransaction(transaction, "Personenkonto1", 10m);
      AddDebitorToTransaction(transaction, "Personenkonto2", 10m);
      AddDebitorToTransaction(transaction, "Personenkonto2.2", 10m);
      Facade.BillTransaction(transaction);

      // not closable (unbalanced) account hierarchie with balanced head account
      SetupAccount("Personenkonto3", "P00003", "PK3", "Buchungskonto", "Personenkonten");

      SetupAccount("Personenkonto3.1", "P00003-1", "PK3.1", "Buchungskonto", "Personenkonto3");
      SetupAccount("Personenkonto3.2", "P00003-2", "PK3.2", "Buchungskonto", "Personenkonto3");

      transaction = PrepareBillTransactionCommand("Test - Head-Balanced Part 1", "11", new DateTime(1999, 1, 1));
      AddCreditorToTransaction(transaction, "Personenkonto3", 10m);
      AddDebitorToTransaction(transaction, "Personenkonto3.1", 5m);
      AddDebitorToTransaction(transaction, "Personenkonto3.2", 5m);
      Facade.BillTransaction(transaction);
      transaction = PrepareBillTransactionCommand("Test - Head-Balanced Part 2", "12", new DateTime(1999, 1, 1));
      AddCreditorToTransaction(transaction, "Personenkonto3.1", 10m);
      AddDebitorToTransaction(transaction, "Personenkonto3", 10m);
      Facade.BillTransaction(transaction);

      // closable (balanced) account hierarchie
      SetupAccount("Personenkonto4", "P00004", "PK4", "Buchungskonto", "Personenkonten");

      SetupAccount("Personenkonto4.1", "P00004-1", "PK4.1", "Buchungskonto", "Personenkonto4");
      SetupAccount("Personenkonto4.2", "P00004-2", "PK4.2", "Buchungskonto", "Personenkonto4");

      transaction = PrepareBillTransactionCommand("Test - Transaction Circle Part 1", "21", new DateTime(1999, 1, 1));
      AddCreditorToTransaction(transaction, "Personenkonto4", 5m);
      AddDebitorToTransaction(transaction, "Personenkonto4.1", 5m);
      Facade.BillTransaction(transaction);
      transaction = PrepareBillTransactionCommand("Test - Transaction Circle Part 2", "22", new DateTime(1999, 1, 1));
      AddCreditorToTransaction(transaction, "Personenkonto4.1", 5m);
      AddDebitorToTransaction(transaction, "Personenkonto4.2", 5m);
      Facade.BillTransaction(transaction);
      transaction = PrepareBillTransactionCommand("Test - Transaction Circle Part 3", "23", new DateTime(1999, 1, 1));
      AddCreditorToTransaction(transaction, "Personenkonto4.2", 5m);
      AddDebitorToTransaction(transaction, "Personenkonto4", 5m);
      Facade.BillTransaction(transaction);
    }

  }
}
