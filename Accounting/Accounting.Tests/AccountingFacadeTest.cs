using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using Accounting.Model;
using System.Linq;

namespace Accounting.Tests
{
  [TestClass]
  public class AccountingFacadeTest : TestBase
  {
    /// <summary>
    /// this test ensures that the accounting facade is correctly generated and dependency injected
    /// </summary>
    [TestMethod]
    public void AccountingFacadeShouldBeCreated()
    {

      var uut = Require<IAccountingFacade>();
      Assert.IsNotNull(uut);
    }


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

    
  }

}