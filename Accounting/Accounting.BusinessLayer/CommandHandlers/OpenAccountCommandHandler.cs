using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer.CommandHandlers
{
  class OpenAccountCommandHandler : ICommandHandler<OpenAccountCommand>
  {
    private IUnitOfWork UnitOfWork;

    public OpenAccountCommandHandler(IUnitOfWork UnitOfWork)
    {
      this.UnitOfWork = UnitOfWork;
    }

    public void Handle(OpenAccountCommand command)
    {
      Account parent = null;
      if (command.ParentAccountId.HasValue)
      {
        parent = UnitOfWork.GetRepository<Account>().GetByID(command.ParentAccountId);
      }


      var account = new Account()
      {
        Name = command.AccountName,
        Number = command.AccountNumber,
        ShortName = command.AccountShortname,
        Parent = parent,
        IsActive = true
      };

      UnitOfWork.GetRepository<Account>().Create(account);

      command.Account = account;

      UnitOfWork.Save();
    }

    public bool Validate(OpenAccountCommand command, out IEnumerable<Exception> ValidationErrors)
    {
      if (command == null) throw new ArgumentNullException("command");

      var errorList = new List<Exception>();
      ValidationErrors = errorList;

      // stage 1: basic property validation
      if (string.IsNullOrWhiteSpace(command.AccountName))
      {
        errorList.Add(new InvalidOperationException("accountname may not be whitespace empty"));
      }
      if (string.IsNullOrWhiteSpace(command.AccountNumber))
      {
        errorList.Add(new InvalidOperationException("acccount number may not be null or empty"));
      }

      if (errorList.Count > 0) return false;
      // stage 2: data integrity validation, if all properties are fine

      if (UnitOfWork.GetRepository<Account>().Get(acc => acc.Name == command.AccountName || acc.Number == command.AccountNumber).Any())
      {
        errorList.Add(new InvalidOperationException("account name or number is not unique"));
      }

      return errorList.Count == 0;
    }
  }
}
