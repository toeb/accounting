using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Owin.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using Accounting.Model;

namespace Application.Web.Test
{
  public class AccountingWebE2ETest : OwinE2ETest
  {
    protected override void InitializeOwin(Owin.IAppBuilder app)
    {
      new Startup().Configuration(app);
    }
  }
}
