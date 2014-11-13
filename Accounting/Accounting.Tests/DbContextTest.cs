using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using Accounting.Model;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Accounting.Tests
{

  public class DbContextTest<TDbContext> :InjectingTestBase where TDbContext : DbContext
  {
    public string ConnectionString
    {
      get
      {
        // todo: get from application config
        return @"Server=.\SQLEXPRESS;Database=Accounting.Testdb;Trusted_connection=true";
      }
    }


    /// <summary>
    /// drops and recreates the database before every test
    /// </summary>
    protected override void Init()
    {
      base.Init();
      Database.SetInitializer(new DropCreateDatabaseAlways<TDbContext>());
      this.Context = new AccountingDbContext();
      if (this.Context.Database.Exists()) Context.Database.Delete();
      this.Context.Database.CreateIfNotExists();
      Container.ComposeExportedValue<DbContext>(Context);

    }



    public AccountingDbContext Context { get; set; }
  }
}
