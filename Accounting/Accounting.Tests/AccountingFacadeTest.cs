using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using Accounting.Model;
using System.Linq;
using System.Collections.Generic;
using Accounting.Tests.Initializers;

namespace Accounting.Tests
{
  [TestClass]
  public class AccountingFacadeTest : TestBase
  {
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
    /// This test ensures that an account can be opened when providing an OpenAccountCommand
    /// </summary>
    [TestMethod]
    public void ShouldOpenAnAccount()
    {
      // arrange
      var uut = Require<IAccountingFacade>();

      var command = new OpenAccountCommand();
      command.AccountName = "my_first_account";
      command.AccountShortname = "shortname";
      command.AccountNumber = "1234";

      // act
      uut.OpenAccountCommandHandler().Handle(command);


      // check the output properties of the command
      Assert.IsNotNull(command.Account);
      Assert.AreNotEqual(0, command.Account.Id);

      var acc = Context.Accounts.Find(command.Account.Id);
      Assert.IsTrue(acc.IsActive);
      Assert.AreEqual(command.AccountName, acc.Name);
      Assert.AreEqual(command.AccountShortname, acc.ShortName);
      Assert.AreEqual(command.AccountNumber, acc.Number);

      var command2 = new OpenAccountCommand()
      {
        AccountName = "my_second_account",
        AccountShortname = "second_shortname",
        AccountNumber = "2345",
        ParentAccountId = acc.Id
      };
      uut.OpenAccountCommandHandler().Handle(command2);

      var acc1 = Context.Accounts.Find(acc.Id);
      var acc2 = Context.Accounts.Find(command2.Account.Id);

      Assert.IsNotNull(acc2.Parent);
      Assert.AreEqual(1, acc1.Children.Count);
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
      uut.OpenAccountCommandHandler().Handle(new OpenAccountCommand() { AccountNumber = "123", AccountName = "asd" });
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
      uut.OpenAccountCommandHandler().Handle(command = new OpenAccountCommand()
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

      uut.UpdateAccountCommandHandler().Handle(updateCommand);

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

      uut.UpdateAccountCommandHandler().Handle(updateCommand);

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
      uut.OpenAccountCommandHandler().Handle(command = new OpenAccountCommand()
      {
        AccountName = "my_account1",
        AccountNumber = "100000"
      });

      uut.OpenAccountCommandHandler().Handle(new OpenAccountCommand()
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

      uut.UpdateAccountCommandHandler().Handle(updateCommand);
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

      uut.CloseAccountCommandHandler().Handle(closeAccountCommand);

      Assert.Fail("Expected exception!");
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

      uut.CloseAccountCommandHandler().Handle(closeAccountCommand);

      Assert.Fail("Expected exception!");
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
      uut.OpenAccountCommandHandler().Handle(open);
      open.Account.IsActive = false;
      Context.Entry(open.Account).State = EntityState.Modified;
      Context.SaveChanges();

      var closeAccountCommand = new CloseAccountCommand()
      {
        AccountId = open.Account.Id,
        Recursive = false
      };

      uut.CloseAccountCommandHandler().Handle(closeAccountCommand);

      Assert.Fail("Expected exception!");
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void CloseAccountRequiresBalancedAccount()
    {
      var uut = Require<IAccountingFacade>();

      SetupTestdataInitialization init = new SetupTestdataInitialization(uut, Context);
      // Use standard test environment setup
      init.SetupAccountingEnvironment();

      uut.CloseAccountCommandHandler().Handle(new CloseAccountCommand()
      {
        AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto2.1").Id
      });

      Assert.Fail("Expected exception!");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void CloseAccountRequiresClosedChildren()
    {
      var uut = Require<IAccountingFacade>();

      SetupTestdataInitialization init = new SetupTestdataInitialization(uut, Context);
      // Use standard test environment setup
      init.SetupAccountingEnvironment();

      uut.CloseAccountCommandHandler().Handle(new CloseAccountCommand()
      {
        AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto3").Id
      });

      Assert.Fail("Expected exception!");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
    public void CloseAccountHierarchieRequiresBalancedChildren()
    {
      var uut = Require<IAccountingFacade>();

      SetupTestdataInitialization init = new SetupTestdataInitialization(uut, Context);
      // Use standard test environment setup
      init.SetupAccountingEnvironment();

      uut.CloseAccountCommandHandler().Handle(new CloseAccountCommand()
      {
        AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto3").Id,
        Recursive = true
      });

      Assert.Fail("Expected exception!");
    }

    [TestMethod]
    public void ShouldCloseBalancedAccountWithoutChildren()
    {
      var uut = Require<IAccountingFacade>();

      SetupTestdataInitialization init = new SetupTestdataInitialization(uut, Context);
      // Use standard test environment setup
      init.SetupAccountingEnvironment();

      Assert.IsTrue(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.1").IsActive);
      uut.CloseAccountCommandHandler().Handle(new CloseAccountCommand()
      {
        AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.1").Id
      });
      Assert.IsFalse(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.1").IsActive);
    }

    [TestMethod]
    public void ShouldRecursivelyCloseBalancedAccountWithBalancedChildren()
    {
      var uut = Require<IAccountingFacade>();

      SetupTestdataInitialization init = new SetupTestdataInitialization(uut, Context);
      // Use standard test environment setup
      init.SetupAccountingEnvironment();

      Assert.IsTrue(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4").IsActive);
      Assert.IsTrue(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.1").IsActive);
      Assert.IsTrue(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.2").IsActive);
      uut.CloseAccountCommandHandler().Handle(new CloseAccountCommand()
      {
        AccountId = Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4").Id,
        Recursive = true
      });
      Assert.IsFalse(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4").IsActive);
      Assert.IsFalse(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.1").IsActive);
      Assert.IsFalse(Context.Accounts.FirstOrDefault(x => x.Name == "Personenkonto4.2").IsActive);
    }

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