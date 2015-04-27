using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Accounting.Service
{


  public class Program
  {
    public static void Main(string[] args)
    {
      CompositionContainer container = new CompositionContainer(new ApplicationCatalog());
      var uri = "http://localhost:9919";
      using (WebApp.Start(uri, new Startup(container).Configuration))
      {
        Trace.WriteLine("listening on " + uri);
        Console.Write("Press Any Key to Quit");
        Console.ReadKey();
      }

    }
  }
}