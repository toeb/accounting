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
  public class InjectingTestBase
  {
    /// <summary>
    /// override this to create a custom container.
    /// defualt implementation creates a composition container using an Application Catalog
    /// </summary>
    /// <returns></returns>
    protected virtual CompositionContainer CreateContainer()
    {
      return new CompositionContainer(new ApplicationCatalog());
    }
  
    [TestInitialize]
    public void InitializeTest()
    {
      Init();
    }
  
    protected virtual void Init()
    {
      Container = CreateContainer();
    }
    
    /// <summary>
    /// Use this method to a instance to a Injected object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Require<T>()
    {
      return Container.GetExport<T>().Value;
    }
  
    public CompositionContainer Container { get; set; }
  }
}
