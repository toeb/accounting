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
  public class AccountingFacadeTest : DbContextTest<AccountingDbContext>
  {
    protected override void AfterDbContextIntialization()
    {
      uut = new AccountingFacade()
      {
        Accounts = new RepositoryBase<Account>(Context)
      };
    }

    [TestMethod]
    public void TestDataConnection()
    {
      Assert.IsTrue(Context.Database.Exists());
    }

    [TestMethod]
    public void OpenAccount()
    {
      // arrange
      var command = new OpenAccountCommand();
      command.AccountName = "my_first_account";
      command.AccountNumber = "1234";
      command.CategoryId = 1;

      // act
      uut.OpenAccount(command);

      // assert
      Assert.IsNotNull(command.Account);
      Assert.AreNotEqual(0, command.Account.Id);
      Context.Accounts.Any(acc => acc.Id == command.Account.Id);

    }


    public IAccountingFacade uut { get; set; }
  }
}
