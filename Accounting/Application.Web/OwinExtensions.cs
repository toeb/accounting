using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using System.IO;
using System.Web.Hosting;
using System.Reflection;

namespace Application.Web
{
  


  public static class OwinExtensions
  {

    public static string MapPath(string path)
    {
      var result = HostingEnvironment.MapPath(path);
      if (result == null)
      {
        var p2 =AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
       var p= Path.GetFullPath("../../../Application.Web");
        result = path.Replace("~", new Uri(p).LocalPath);
        result = Path.GetFullPath(result);
      }
      return result;
    }
    public static void ServeVirtualDirectory(this IAppBuilder app, string requestPath, string physicalPath)
    {

      PathString requestPathString;
      if (string.IsNullOrWhiteSpace(requestPath)) requestPathString = PathString.Empty;
      else requestPathString = new PathString(requestPath);

      physicalPath = MapPath(physicalPath);
      var fileSystem = new PhysicalFileSystem(physicalPath);

      {
        var options = new FileServerOptions()
        {
          EnableDefaultFiles = true,
          RequestPath = requestPathString,
          FileSystem = fileSystem
        };
        app.UseFileServer(options);
      }
    }

  }
}
