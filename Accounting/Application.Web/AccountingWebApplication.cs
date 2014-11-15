using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Application.Web
{
  public class AccountingWebApplication : AccountingApplication
  {
    protected override CompositionContainer CreateContainer()
    {
      var container = base.CreateContainer();
      container.ComposeExportedValue<DbContext>(new AccountingDbContext());
      return container;
    }
  
  }
}
