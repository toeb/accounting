using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  [Export(typeof(IAccountingFacade))]
  public class AccountingFacade : IAccountingFacade
  {
    [Import]
    public IRepository<Account> Accounts { get; set; }


    public void OpenAccount(OpenAccountCommand command)
    {

      if (string.IsNullOrWhiteSpace(command.AccountName)) throw new InvalidOperationException("accountname may not be whitespace empty");
      if (string.IsNullOrWhiteSpace(command.AccountNumber)) throw new InvalidOperationException("acccount number may not be null or empty");

      if (Accounts.Get(acc => acc.Name == command.AccountName || acc.Number == command.AccountNumber).Any()) throw new InvalidOperationException("account name or number is not unique");

      Account parent = null;
      if(command.ParentAccountId.HasValue){
        parent =Accounts.GetByID(command.ParentAccountId);
      }


      var account = new Account()
      {
        Name = command.AccountName,
        Number = command.AccountNumber,
        Parent = parent
      };

      Accounts.Insert(account);

      command.Account = account;


    }
  }
}
