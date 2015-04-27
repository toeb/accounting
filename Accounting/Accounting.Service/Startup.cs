using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Net.Http;
using Accounting.Service.Utility;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Owin.Cors;
using Accounting.Service;


[assembly: OwinStartup(typeof(Startup))]

namespace Accounting.Service
{
  public partial class Startup
  {
    public Startup(){
      this.Container = new CompositionContainer(new ApplicationCatalog());
    }

    public Startup(CompositionContainer container)
    {
      this.Container = container;
    }
    public void Configuration(IAppBuilder app)
    {
      var config = new HttpConfiguration();
      config.DependencyResolver = new MefDependencyResolver(Container);


      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      config.Routes.MapHttpRoute(
          name: "ActionApi",
          routeTemplate: "api/{controller}/{action}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      // enable cross origin scripting
      app.UseCors(new CorsOptions() { });
      // use webapi
      app.UseWebApi(config);
    }

    public CompositionContainer Container { get; set; }
  }
}