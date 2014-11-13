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

  public class DbContextTest<TDbContext> where TDbContext : DbContext
  {
    public string ConnectionString
    {
      get
      {
        
        return @"Server=.\SQLEXPRESS;Database=Accounting.Testdb;Trusted_connection=true";
      }
    }
  
    [TestInitialize]
    public void Init()
    {
      Database.SetInitializer(new DropCreateDatabaseAlways<TDbContext>());
      this.Context = new AccountingDbContext();
      this.Context.Database.CreateIfNotExists();
      AfterDbContextIntialization();
  
    }
  
    protected virtual void AfterDbContextIntialization()
    {
    }
  
  
  
    public AccountingDbContext Context { get; set; }
  }
}
