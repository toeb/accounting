using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using System.IO;
using System.Web.Hosting;


[assembly: OwinStartup(typeof(Application.Web.Startup))]


namespace Application.Web
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      app.ServeVirtualDirectory("/bower_components", "~/Frontend/bower_components");
      app.ServeVirtualDirectory("", "~/Frontend/dist/");
      app.ServeVirtualDirectory("", "~/Frontend/src/");

      
      // Configure Web API for self-host. 
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      app.UseWebApi(config); 
    }
  }
}
