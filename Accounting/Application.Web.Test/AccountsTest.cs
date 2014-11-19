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
  [TestClass]
  public class AccountsTest : AccountingWebE2ETest
  {
    protected override RemoteWebDriver CreateWebDriver()
    {
      return new ChromeDriver();  
    }
    [TestMethod]
    public void MyTest()
    {
      Context.Accounts.Add(new Account() { Name ="asd"});
      Context.SaveChanges();
      Browser.Navigate().GoToUrl(Url+"/#accounts");
      Browser.FindElementById("getAccountsBtn").Click();
      var res2 = Browser.FindElementById("testid").Text;
      var res = Browser.FindElementById("accountscontainer").Text;
    }

    

  }
}
