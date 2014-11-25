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
  public class AccountingDataContextTest : TestBase
  {
  
    [TestMethod]
    public void TestDataConnection()
    {
      var uut = Require<DbContext>();
      Assert.IsInstanceOfType(uut, typeof(AccountingDbContext));
      Assert.IsTrue(uut.Database.Exists());
    }
  
  }
}
