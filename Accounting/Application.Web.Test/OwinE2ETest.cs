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

namespace Application.Web.Test
{
  public class OwinE2ETest : Accounting.Tests.AccountingDataContextTest
  {
    public RemoteWebDriver Browser { get; private set; }
  
    protected virtual string GetUrl()
    {
      return "http://localhost:9286";
    }

    protected virtual RemoteWebDriver CreateWebDriver()
    {
      return new PhantomJSDriver();
    }
  
    //[TestInitialize]
    //public void InitializeTest()
    //{
    //  Init();
    //}

    protected override void Init()
    {
      base.Init();
      this.Url = GetUrl();
      this.WebApplication = WebApp.Start(Url, app =>
      {
        InitializeOwin(app);
      });
      this.Browser = CreateWebDriver();
    }
  
    protected virtual void InitializeOwin(Owin.IAppBuilder app)
    {
  
    }
  
    [TestCleanup]
    public void CleanupTest()
    {
      WebApplication.Dispose();
    }
  
    public string Url { get; set; }
  
    public IDisposable WebApplication { get; set; }
  }
}
