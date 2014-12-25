using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using Accounting.Model;
using System.Linq;
using System.Collections.Generic;

namespace Accounting.Tests
{
  [TestClass]
  public class AccountingFacadeTest : TestBase
  {

    #region Test Setup for advanced Tests

    /// <summary>
    /// <para>Initializes the database with some entries for more advanced tests. Available accounts in hierarchie:</para>
    /// <para>|Wertkonten| -> |Konto|</para>
    /// <para>|Sachkonten| -> |Sachkonto1|</para>
    /// <para>|Sachkonten| -> |Sachkonto2|</para>
    /// <para>|Sachkonten| -> |Sachkonto3|</para>
    /// <para>|Personenkonten| -> |Personenkonto1|</para>
    /// <para>|Personenkonten| -> |Personenkonto2|</para>
    /// <para>|Personenkonten| -> |Personenkonto2| -> |Personenkonto2.1|</para>
    /// <para>|Personenkonten| -> |Personenkonto2| -> |Personenkonto2.2|</para>
    /// <para>Account Activity and Balance for each account:</para>
    /// <para>|konto| = credit: 0 | debit: 30.5 | balance: -30.5</para>
    /// <para>|Sachkonto1| = credit: 5.5 | debit: 0 | balance: 5.5</para>
    /// <para>|Sachkonto2| = credit: 0 | debit: 0 | balance: 0</para>
    /// <para>|Personenkonto1| = credit: 25 | debit: 10 | balance: 15</para>
    /// <para>|Personenkonto2| = credit: 15 | debit: 10 | balance: 5</para>
    /// <para>|Personenkonto2.1| = credit: 15 | debit: 0 | balance: 15</para>
    /// <para>|Personenkonto2.2| = credit: 0 | debit: 10 | balance: -10</para>
    /// </summary>
    protected void SetupAccountingEnvironment()
    {
      var uut = Require<IAccountingFacade>();

      Context.AccountCategories.Add(new AccountCategory() { Name = "Hauptkategorie" });
      Context.AccountCategories.Add(new AccountCategory() { Name = "Unterkategorie" });
      Context.AccountCategories.Add(new AccountCategory() { Name = "Buchungskonto" });

      Context.SaveChanges();

      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Wertkonten",
        AccountNumber = "#00001",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Hauptkategorie").Id
      });
      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Sachkonten",
        AccountNumber = "#00002",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Hauptkategorie").Id
      });
      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Personenkonten",
        AccountNumber = "#00003",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Hauptkategorie").Id
      });

      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Konto",
        AccountNumber = "K00001",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name=="Wertkonten").Id
      });

      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Sachkonto1",
        AccountNumber = "S00001",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Sachkonten").Id
      });
      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Sachkonto2",
        AccountNumber = "S00002",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Sachkonten").Id
      });

      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Personenkonto1",
        AccountNumber = "P00001",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonten").Id
      });
      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Personenkonto2",
        AccountNumber = "P00002",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonten").Id
      });

      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Personenkonto2.1",
        AccountNumber = "P00002-1",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2").Id
      });
      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "Personenkonto2.1",
        AccountNumber = "P00002-2",
        CategoryId = Context.AccountCategories.FirstOrDefault(x => x.Name == "Buchungskonto").Id,
        ParentAccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2").Id
      });

      var creditors1 = new AddPartialTransactionCommand[] {
        new AddPartialTransactionCommand() { Amount = 5.5m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Sachkonto1").Id },
        new AddPartialTransactionCommand() { Amount = 10m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2").Id },
        new AddPartialTransactionCommand() { Amount = 15m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2.1").Id }
      };

      var debitors1 = new AddPartialTransactionCommand[] {
        new AddPartialTransactionCommand() { Amount = 30.5m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Konto").Id }
      };

      uut.BillTransaction(new BillTransactionCommand()
      {
        TransactionText = "Test - Zahlungseingang mit split",
        Receipt = "001",
        ReceiptDate = new DateTime(1999, 1, 20),
        Credits = new List<AddPartialTransactionCommand>(creditors1),
        Debits = new List<AddPartialTransactionCommand>(debitors1)
      });


      var creditors2 = new AddPartialTransactionCommand[] {
        new AddPartialTransactionCommand() { Amount = 25m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto1").Id },
        new AddPartialTransactionCommand() { Amount = 5m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2").Id }
      };

      var debitors2 = new AddPartialTransactionCommand[] {
        new AddPartialTransactionCommand() { Amount = 10m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto1").Id },
        new AddPartialTransactionCommand() { Amount = 10m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2").Id },
        new AddPartialTransactionCommand() { Amount = 10m, AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2.2").Id }
      };

      uut.BillTransaction(new BillTransactionCommand()
      {
        TransactionText = "Test - Many to Many transaction",
        Receipt = "002",
        ReceiptDate = new DateTime(1999, 2, 1),
        Credits = new List<AddPartialTransactionCommand>(creditors2),
        Debits = new List<AddPartialTransactionCommand>(debitors2)
      });
    }

    #endregion

    #region Test Requirements
    /// <summary>
    /// this test ensures that the accounting facade is correctly generated and dependency injected
    /// </summary>
    [TestMethod]
    public void AccountingFacadeShouldBeCreated()
    {

      var uut = Require<IAccountingFacade>();
      Assert.IsNotNull(uut);
    }

    #endregion

    #region OpenAccountCommand

    /// <summary>
    /// This test ensures that an account can be opened when specifying an accountname and number
    /// </summary>
    [TestMethod]
    public void ShouldOpenAnAccountWithNameAndNumber()
    {
      // arrange
      var uut = Require<IAccountingFacade>();

      var command = new OpenAccountCommand();
      command.AccountName = "my_first_account";
      command.AccountNumber = "1234";

      // act
      uut.OpenAccount(command);


      // assert
      Assert.IsNotNull(command.Account);
      Assert.AreNotEqual(0, command.Account.Id);
      Assert.IsTrue(command.Account.IsActive);
      Context.Accounts.Any(acc => acc.Id == command.Account.Id);

    }


    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void OpenAccountShouldFailIfAccountNumberAlreadyExists()
    {
      //arrange
      var uut = Require<IAccountingFacade>();
      var uow = Require<IUnitOfWork>();

      var accounts = Require<IRepository<Account>>();
      accounts.Create(new Account() { 
        Number = "123"
      });

      uow.Save();

      // act
      uut.OpenAccount(new OpenAccountCommand() { AccountNumber = "123", AccountName = "asd" });
    }

    #endregion

    #region UpdateAccountCommand

    /// <summary>
    /// This test ensures that an account can be updated when specifying an accountname, shortname or number
    /// </summary>
    [TestMethod]
    public void ShouldUpdateAnAccount()
    {
      // arrange
      var uut = Require<IAccountingFacade>();

      OpenAccountCommand command;
      uut.OpenAccount(command = new OpenAccountCommand()
      {
        AccountName = "my_updatable_account",
        AccountNumber = "100000"
      });

      Assert.IsNotNull(command.Account);
      Assert.AreNotEqual(0, command.Account.Id);

      var id = command.Account.Id;

      var updateCommand = new UpdateAccountCommand()
      {
        AccountId = command.Account.Id,
        NewShortName = "updated_account"
      };

      uut.UpdateAccount(updateCommand);

      // assert
      Assert.IsNotNull(updateCommand.ModifiedAccount);
      Assert.AreEqual(id, updateCommand.ModifiedAccount.Id);
      Assert.AreEqual("100000", updateCommand.ModifiedAccount.Number);
      Assert.AreEqual("my_updatable_account", updateCommand.ModifiedAccount.Name);
      Assert.AreEqual("updated_account", updateCommand.ModifiedAccount.ShortName);

      updateCommand = new UpdateAccountCommand()
      {
        AccountId = command.Account.Id,
        NewShortName = "updated_account2",
        NewName = "my_account_name",
        NewNumber = "100001"
      };

      uut.UpdateAccount(updateCommand);

      // assert
      Assert.IsNotNull(updateCommand.ModifiedAccount);
      Assert.AreEqual(id, updateCommand.ModifiedAccount.Id);
      Assert.AreEqual("100001", updateCommand.ModifiedAccount.Number);
      Assert.AreEqual("my_account_name", updateCommand.ModifiedAccount.Name);
      Assert.AreEqual("updated_account2", updateCommand.ModifiedAccount.ShortName);
    }

    /// <summary>
    /// This test ensures that an account update is rejected when there is a parameter conflict
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void ShouldValidateParametersOnAccountUpdate()
    {
      // arrange
      var uut = Require<IAccountingFacade>();

      OpenAccountCommand command;
      uut.OpenAccount(command = new OpenAccountCommand()
      {
        AccountName = "my_account1",
        AccountNumber = "100000"
      });

      uut.OpenAccount(new OpenAccountCommand()
      {
        AccountName = "my_account2",
        AccountNumber = "200000"
      });

      Assert.IsNotNull(command.Account);
      Assert.AreNotEqual(0, command.Account.Id);

      var id = command.Account.Id;

      // try duplicate account number
      var updateCommand = new UpdateAccountCommand()
      {
        AccountId = command.Account.Id,
        NewNumber = "200000"
      };

      uut.UpdateAccount(updateCommand);
    }

    #endregion

    #region CloseAccountCommand

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), AllowDerivedTypes=true)]
    public void CloseAccountRequiresAccountId()
    {
      var closeAccountCommand = new CloseAccountCommand()
      {
        AccountId = 0,
        Recursive = false
      };
      var uut = Require<IAccountingFacade>();

      uut.CloseAccount(closeAccountCommand);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void CloseAccountRequiresValidAccount()
    {
      var closeAccountCommand = new CloseAccountCommand()
      {
        AccountId = 100,
        Recursive = false
      };
      var uut = Require<IAccountingFacade>();

      // sanity check: element should not exist in database
      Assert.IsFalse(Context.Accounts.Any(x => x.Id == 100));

      uut.CloseAccount(closeAccountCommand);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void CloseAccountRequiresOpenAccount()
    {
      var uut = Require<IAccountingFacade>();
      var open = new OpenAccountCommand()
      {
        AccountName = "A",
        AccountNumber = "1000",
        CategoryId = 0
      };
      uut.OpenAccount(open);
      open.Account.IsActive = false;
      Context.Entry(open.Account).State = EntityState.Modified;
      Context.SaveChanges();

      var closeAccountCommand = new CloseAccountCommand()
      {
        AccountId = open.Account.Id,
        Recursive = false
      };

      uut.CloseAccount(closeAccountCommand);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void CloseAccountRequiresBalancedAccount()
    {
      // Use standard test environment setup
      SetupAccountingEnvironment();

      var uut = Require<IAccountingFacade>();

      uut.CloseAccount(new CloseAccountCommand()
      {
        AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2.1").Id
      });
    }

    // TODO positive tests

    #endregion

    #region BillTransactionCommand+Extensions

    [TestMethod]
    public void BillTransactionShouldWorkForABalancedTransactionWithAllData()
    {
      var tobi = new Account() { IsActive = true, Name = "Tobi" };
      var flo = new Account() { IsActive = true, Name = "Flo" };
      var matthias = new Account() { IsActive = true, Name = "Matthi" };
      var uow = Require<IUnitOfWork>();
      uow.GetRepository<Account>().Create(tobi);
      uow.GetRepository<Account>().Create(flo);
      uow.GetRepository<Account>().Create(matthias);
      uow.Save();

      var uut = Require<IAccountingFacade>();

      var cmd = new BillTransactionCommand()
      {
        Receipt = "My Receipt",
        ReceiptDate = DateTime.Now,
        TransactionText = "billing something"
      };
      cmd.AddCreditor(2, tobi.Id);
      cmd.AddCreditor(3, flo.Id);
      cmd.AddDebitor(5, matthias.Id);


      uut.BillTransaction(cmd);

      Assert.AreEqual(3, Context.Set<PartialTransaction>().Count());
      Assert.AreEqual(1, Context.Set<Transaction>().Count());
    }

    #endregion

    #region RevertTransactionCommand

    /// <summary>
    /// this test ensures that a storno creates another transaction whith inverted amounts
    /// and sets the correct fields
    /// </summary>
    [TestMethod]
    public void ShouldRevertTransaction()
    {
      // arrange
      var uut = Require<IAccountingFacade>();
      var transactions =  Require<IUnitOfWork>().GetRepository<Transaction>();
      var tobi = uut.OpenAccount("Tobi", "1").Account;
      var flo= uut.OpenAccount("Flo", "2").Account;

      var transaction = uut.BillTransaction(tobi, flo, 10).Transaction;

      // act

      var cmd = new RevertTransactionCommand()
      {
       TransactionId = transaction.Id 
      };

      uut.RevertTransaction(cmd);

      // assert

      Assert.IsNotNull(cmd.RevertedTransaction);
      // reverted transactions storno should be transaction itself
      Assert.AreEqual(cmd.RevertedTransaction, cmd.RevertedTransaction.Storno.Storno);
      Assert.AreEqual(transaction.Id, cmd.RevertedTransaction.Storno.Id);
      // only negative amounts
      Assert.IsTrue(cmd.RevertedTransaction.Partials.All(p=>p.Amount == -10m));

      Assert.IsTrue(transactions.Read().Count() == 2);

    }

    #endregion

    #region ListAccountCommand+Extensions

    [TestMethod]
    public void ShouldListNoAccountsWhenDbIsEmpty()
    {
      var uut = Require<IAccountingFacade>();
      var result = uut.QueryAccounts().Count();
      Assert.AreEqual(0, result);
    }


    [TestMethod]
    public void ShouldListActiveAccount()
    {
      var uut = Require<IAccountingFacade>();

      uut.OpenAccount("toeb", "123");
      uut.OpenAccount("toeb2", "234");

      var cmd = new ListAccountsCommand();
      uut.ListAccounts(cmd);

      Assert.IsNotNull(cmd.Query);
      Assert.AreEqual(2, cmd.Query.Count());
      Assert.IsTrue(cmd.Query.Any(acc => acc.Name == "toeb"));
      Assert.IsTrue(cmd.Query.Any(acc => acc.Name == "toeb2"));
    }

    #endregion
  }

}