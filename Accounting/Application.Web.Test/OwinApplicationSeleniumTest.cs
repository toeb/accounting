﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Owin.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;

namespace Application.Web.Test
{
  public class AccountingWebE2ETest : OwinE2ETest
  {
    protected override void InitializeOwin(Owin.IAppBuilder app)
    {
      new Startup().Configuration(app);
    }
  }
  public class OwinE2ETest
  {
    public IWebDriver Browser { get;private set; }

    protected virtual string GetUrl()
    {
      return "http://localhost:9286";
    }

    protected virtual IWebDriver CreateWebDriver()
    {
      return new PhantomJSDriver();
    }

    [TestInitialize]
    public void InitializeTest()
    {
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

  [TestClass]
  public class OwinApplicationSeleniumTests
  {
    [TestMethod]
    public async Task UseOwinTestServer()
    {
      // this seems to create a in memory server of the owin app.
      using (var server = TestServer.Create<Startup>())
      {
        var client = new HttpClient(server.Handler);
        var result = await client.GetAsync("http://localhost:9876");
        var data = await result.Content.ReadAsStringAsync();

      }
    }

    [TestMethod]
    public async Task UseWebApp()
    {
      string url = "http://localhost:8391";
      using (WebApp.Start<Startup>(url))
      {

        var client = new HttpClient();
        var result = await client.GetAsync(url);
        var data = await result.Content.ReadAsStringAsync();
      }
    }

    [TestMethod]
    public async Task UseChromeWebDriver()
    {
      string url = "http://localhost:8391";
      using (WebApp.Start<Startup>(url))
      {
        using (var browser = new ChromeDriver())
        {
          browser.Navigate().GoToUrl(url);
          var t = browser.Title;
          Assert.AreEqual("frontend", t);
        }
      }
    }
    [TestMethod]
    public async Task UsePhantomJsDriver()
    {
      string url = "http://localhost:8391";
      using (WebApp.Start<Startup>(url))
      {
        using (var browser = new PhantomJSDriver())
        {
          browser.Navigate().GoToUrl(url);
          var t = browser.Title;
          Assert.AreEqual("frontend", t);
        }
      }
    }

    [TestMethod]
    public async Task UseIETestDriver()
    {
      string url = "http://localhost:8391";
      using (WebApp.Start<Startup>(url))
      {
        using (var browser = new OpenQA.Selenium.IE.InternetExplorerDriver())
        {
          browser.Navigate().GoToUrl(url);
          
          var t = browser.Title;
          Assert.AreEqual("frontend", t);
        }
      }
    }
  }
}