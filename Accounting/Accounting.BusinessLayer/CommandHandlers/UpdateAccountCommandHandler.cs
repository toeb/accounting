using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer.CommandHandlers
{
  class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountCommand>
  {
    private IUnitOfWork UnitOfWork;

    public UpdateAccountCommandHandler(IUnitOfWork UnitOfWork)
    {
      this.UnitOfWork = UnitOfWork;
    }

    public void Handle(UpdateAccountCommand command)
    {
      var account = UnitOfWork.GetRepository<Account>().GetByID(command.AccountId);

      if (command.NewName != null)
      {
        if (UnitOfWork.GetRepository<Account>().Get(acc => acc.Name == command.NewName).Any()) throw new InvalidOperationException("account name is not unique");
        account.Name = command.NewName;
      }

      if (command.NewShortName != null)
      {
        if (UnitOfWork.GetRepository<Account>().Get(acc => acc.ShortName == command.NewShortName).Any()) throw new InvalidOperationException("account shortname is not unique");
        account.ShortName = command.NewShortName;
      }

      if (command.NewNumber != null)
      {
        if (UnitOfWork.GetRepository<Account>().Get(acc => acc.Number == command.NewNumber).Any()) throw new InvalidOperationException("account number is not unique");
        account.Number = command.NewNumber;
      }

      UnitOfWork.GetRepository<Account>().Update(account);

      command.ModifiedAccount = account;

      UnitOfWork.Save();
    }

    public bool Validate(UpdateAccountCommand command, out IEnumerable<Exception> ValidationErrors)
    {
      var errors = new List<Exception>();
      if (command == null)
      {
        errors.Add(new ArgumentNullException("command"));
      }
      if (command.AccountId <= 0)
      {
        errors.Add(new ArgumentException("command.AccountId"));
      }
      if (command.ModifiedAccount != null)
      {
        errors.Add(new ArgumentException("command.ModifiedAccount expected to be null"));
      }
      if (command.NewName != null && string.IsNullOrWhiteSpace(command.NewName))
      {
        errors.Add(new InvalidOperationException("account name may not be whitespace or empty"));
      }
      if (command.NewShortName != null && string.IsNullOrWhiteSpace(command.NewShortName))
      {
        errors.Add(new InvalidOperationException("acccount number may not be whitespace or empty"));
      }
      if (command.NewNumber != null && string.IsNullOrWhiteSpace(command.NewNumber))
      {
        errors.Add(new InvalidOperationException("acccount number may not be whitespace or empty"));
      }
      ValidationErrors = errors.Count == 0 ? null : errors;
      return errors.Count == 0;
    }
  }
}
