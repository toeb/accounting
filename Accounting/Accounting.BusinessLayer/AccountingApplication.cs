using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class AccountingApplication :IDisposable
  {
    public AccountingApplication()
    {
      ApplicationContainer = CreateContainer();
    }

    protected virtual CompositionContainer CreateContainer()
    {
      return new CompositionContainer(new ApplicationCatalog());
    }



    CompositionContainer ApplicationContainer { get; set; }


    public T Require<T>()
    {
      return ApplicationContainer.GetExport<T>().Value;
    }



    public void Dispose()
    {
      ApplicationContainer.Dispose();
    }
  }
}
