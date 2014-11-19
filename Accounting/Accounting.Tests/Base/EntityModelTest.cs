using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using Accounting.Model;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace Accounting.Tests
{
  [TestClass]
  public class EntityModelTest : TestBase
  {
  
    [TestMethod]
    public void ShouldSaveAccountCorrectly()
    {
      var uut = Require<DbContext>();

      // create account with category


      var account = new Account() { 
        Name="Asd",
        Number="1",
        ShortName="b",
        AccountCategory = new AccountCategory()
        {
          Name="Default"
        },
        Parent = new Account()
        {
          Name="b",
          Number="2",
          ShortName="c"
          
        }

      };
      account.Parent.AccountCategory = account.AccountCategory;

      uut.Entry(account).State = EntityState.Added;
      uut.SaveChanges();

      var context2 = CreateContext();


      var savedAccount = uut.Set<Account>().Single(acc=>acc.Name =="Asd");

      
      Assert.IsNotNull(savedAccount.AccountCategory);
      Assert.AreNotEqual(0,savedAccount.Id);
      Assert.AreEqual("Asd", savedAccount.Name);
      Assert.AreEqual("1", savedAccount.Number);
      Assert.AreEqual("b", savedAccount.ShortName);
      Assert.IsNotNull(savedAccount.Parent);
      Assert.IsNotNull(savedAccount.Parent.Children);
      Assert.IsTrue(savedAccount.Parent.Children.Contains(savedAccount));

    }

    [TestMethod]
    public void MetaShouldCorrectlyInherit()
    {
      var uut = Require<DbContext>();
      uut.Entry(new Account() { Name = "a" }).State = EntityState.Added;
      uut.Entry(new Account() { Name = "b" }).State = EntityState.Added;
      uut.Entry(new AccountCategory() { Name = "c" }).State = EntityState.Added;

      uut.SaveChanges();

      var context2 = CreateContext();

      var metas = uut.Set<Meta>().ToArray();
      var accounts = uut.Set<Account>().ToArray();
      var cat = uut.Set<AccountCategory>().ToArray();

      

      Assert.AreEqual(3, metas.Count());
      Assert.AreEqual(2, metas.OfType<Account>().Count());
      Assert.AreEqual(1, metas.OfType<AccountCategory>().Count());
      Assert.AreEqual(cat.Single(), metas.OfType<AccountCategory>().Single());
    }

    [TestMethod]
    public void TransactionsShouldCorrectlyBeCreatedWithPartials()
    {
      var transactions = Require<IRepository<Transaction>>();

      var t1 = new Transaction()
      {
        Text = "asd",
        ReceiptNumber = "bsd",
        ReceiptDate = DateTime.Now

      };
      t1.Partials.Add(new PartialTransaction()
      {
        Amount = 3,
        Type = PartialTransactionType.Credit,
        Account = new Account() { Name = "a" }
      });
      t1.Partials.Add(new PartialTransaction()
      {
        Amount = 4,
        Type = PartialTransactionType.Credit,
        Account = new Account() { Name = "a" }
      });
      t1.Partials.Add(new PartialTransaction()
      {
        Amount = 3,
        Type = PartialTransactionType.Debit,
        Account = new Account() { Name = "a" }
      });

      transactions.Insert(t1);
      Context.SaveChanges();


      var context2 = CreateContext();

      var t2 = context2.Transactions.Single();

      Assert.AreEqual(3,t2.Partials.Count());

      foreach (var p in t2.Partials)
      {
        Assert.IsTrue(p.Transaction == t2);
      }

      

    }
  }
}
