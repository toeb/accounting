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
    [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes=true)]
    public void OpenAccountShouldFailIfAccountNumberAlreadyExists()
    {
      //arrange
      var uut = Require<IAccountingFacade>();
      var uow = Require<IUnitOfWork>();

      var accounts = Require<IRepository<Account>>();
      accounts.Insert(new Account() { 
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
      var matthias= new Account() { IsActive = true, Name = "Matthi" };
      var uow = Require<IUnitOfWork>();
      uow.GetRepository<Account>().Insert(tobi);
      uow.GetRepository<Account>().Insert(flo);
      uow.GetRepository<Account>().Insert(matthias);
      uow.Save();
      
      var uut = Require<IAccountingFacade>();

      var cmd = new BillTransactionCommand()
      {
        Receipt = "My Receipt",
        ReceiptDate = DateTime.Now,
        TransactionText = "billing something"
      };
      cmd.PartialTransactions.Add(new PartialTransaction()
      {
        Amount = 2,
        Account = new Account() { Id = tobi.Id },
        Type = PartialTransactionType.Credit
      });
      cmd.PartialTransactions.Add(new PartialTransaction()
      {
        Amount = 3,
        Account = new Account() { Id = flo.Id },
        Type = PartialTransactionType.Credit
      });
      cmd.PartialTransactions.Add(new PartialTransaction()
      {
        Amount = 5,
        Account = new Account() { Id = matthias.Id },
        Type = PartialTransactionType.Debit
      });


      uut.BillTransaction(cmd);

      Assert.AreEqual(3, Context.Set<PartialTransaction>().Count());
      Assert.AreEqual(1, Context.Set<Transaction>().Count());
    }

  }
}
