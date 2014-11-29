﻿using System;
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
  }

}