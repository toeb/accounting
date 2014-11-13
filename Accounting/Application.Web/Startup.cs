using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Application.Web.Startup))]

namespace Application.Web
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
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
