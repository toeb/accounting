using Accounting.DataLayer;
using Accounting.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Tests
{
  [TestClass]
  public class UnitOfWorkTest : TestBase
  {
    [TestMethod]
    public void ShouldCreateInstance()
    {
      var uut = Require<IUnitOfWork>();

      Assert.IsNotNull(uut);

      Assert.AreEqual(0, uut.AccountRepository.Get().Count());

      var account = new Account();
      uut.AccountRepository.Insert(account);
      uut.Save();

      var context = Require<DbContext>();
      Assert.IsTrue(context.Entry(account).State == EntityState.Unchanged);
    }
  }
}
