using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web.Http.Dependencies;

namespace Accounting.Service.Utility
{
  /// <summary>
  /// The MefDependency Resolver is a plugin for the WebApi Framework 
  /// and is a Injection Container which can create controllers for requests.
  /// Controllers need to have the PartCreationPolicy.NonShared or they will not 
  /// be created per request and thus cause errors
  /// 
  /// </summary>
  public class MefDependencyResolver : IDependencyResolver
  {
    public MefDependencyResolver(CompositionContainer container)
    {
      this.Container = container;
    }
    public IDependencyScope BeginScope()
    {
      return this;
    }


    public object GetService(Type serviceType)
    {
      if (serviceType == null) throw new ArgumentNullException("serviceType");
      var name = AttributedModelServices.GetContractName(serviceType);
      var export = Container.GetExportedValueOrDefault<object>(name);
      if (export == null) return null;
      return export;
    }


    public IEnumerable<object> GetServices(Type serviceType)
    {
      if (serviceType == null) throw new ArgumentNullException("serviceType");
      var exports = Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
      return exports;
    }


    public void Dispose()
    {
    }

    public CompositionContainer Container { get; set; }
  }
}
