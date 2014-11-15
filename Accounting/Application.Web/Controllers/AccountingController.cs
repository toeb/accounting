using Accounting.BusinessLayer;
using Accounting.DataLayer;
using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Application.Web.Controllers
{

    public class AccountingController : ApiController
    {

      public AccountingController()
      {
        var application = new AccountingWebApplication();
        this.AccountingFacade = application.Require<IAccountingFacade>();
        this.Accounts = application.Require<IRepository<Account>>();


        
      }


      [ActionName("GetAccounts")]
      public IQueryable<Account> GetAccounts()
      {
        return Accounts.Get().AsQueryable();
        //return new[] { new Account() { Name = "Account1", Id = 2 }, new Account() { Name = "Account2", Id = 3 } }.AsQueryable();
      }


      [ActionName("OpenAccount")]
      [HttpPost]
      public async Task<OpenAccountCommand> OpenAccount([FromBody] OpenAccountCommand command)
      {
        AccountingFacade.OpenAccount(command);

        return command;
      }


      public IAccountingFacade AccountingFacade { get; set; }

      public IRepository<Account> Accounts { get; set; }
    }
}
