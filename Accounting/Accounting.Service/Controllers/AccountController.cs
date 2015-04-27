using Accounting.BusinessLayer;
using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Accounting.Service.Controllers
{
  [Export(typeof(AccountController))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccountController : ApiController
  {
    [ImportingConstructor]
    public AccountController()
    {
      //this.Accounting = accounting;
      //this.Accounts = accounts;
    }

    public IEnumerable<Account> GetAll()
    {
      return Accounts.Get();
    }

    public Account GetById(int id)
    {
      return Accounts.GetByID(id);
    }

    public OpenAccountCommand OpenAccount([FromBody] OpenAccountCommand command)
    {
      Accounting.OpenAccount(command);
      return command;
    }
    public CloseAccountCommand CloseAccount([FromBody] CloseAccountCommand command)
    {
      Accounting.CloseAccount(command);
      return command;
    }
    
    
    public IAccountingFacade Accounting { get; set; }

    public IRepository<Account> Accounts { get; set; }
  }
}