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
  public class TestBase :DbContextTest<AccountingDbContext>
  {

  }
}
