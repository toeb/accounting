﻿using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer.CommandHandlers
{
  class CloseAccountCommandHandler : ICommandHandler<CloseAccountCommand>
  {
    private IUnitOfWork UnitOfWork;

    public CloseAccountCommandHandler(IUnitOfWork UnitOfWork)
    {
      this.UnitOfWork = UnitOfWork;
    }

    public void Handle(CloseAccountCommand command)
    {
      var AccToClose = UnitOfWork.GetRepository<Account>().GetByID(command.AccountId);

      if (command.Recursive)
      {
        CheckAccountsForCloseRecursive(AccToClose);
        CloseAccountsRecursive(AccToClose);
      }
      else
      {
        CheckAccountForClose(AccToClose);
        // FIX: empty children-list may be null instead
        if (AccToClose.Children != null && AccToClose.Children.Any(x => x.IsActive)) throw new InvalidOperationException("Account " + AccToClose.Id + " has active children and the close command was not called recursively!");
        CloseAccountInternal(AccToClose);
      }
      command.ClosedAccount = AccToClose;

      UnitOfWork.Save();
    }

    public bool Validate(CloseAccountCommand command, out IEnumerable<Exception> ValidationErrors)
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
      if (command.ClosedAccount != null)
      {
        errors.Add(new ArgumentException("command.ClosedAccount expected to be null"));
      }

      // only query the database, after primitive validation succeeded
      if (errors.Count == 0)
      {
        var AccToClose = UnitOfWork.GetRepository<Account>().GetByID(command.AccountId);
        if (AccToClose == null)
        {
          errors.Add(new InvalidOperationException("The specified Account does not exist!"));
        }
        else if (!AccToClose.IsActive)
        {
          errors.Add(new InvalidOperationException("The specified Account is already closed!"));
        }
      }

      ValidationErrors = errors.Count == 0 ? null : errors;
      return errors.Count == 0;
    }

    #region Helper methods

    // checks balance for specified account, requires 0 balance
    private void CheckAccountForClose(Account account)
    {
      var TransRepo = UnitOfWork.GetRepository<PartialTransaction>();
      decimal Credit = TransRepo.Get(x => x.Account.Id == account.Id && x.Type == PartialTransactionType.Credit).Sum(x => x.Amount);
      decimal Debit = TransRepo.Get(x => x.Account.Id == account.Id && x.Type == PartialTransactionType.Debit).Sum(x => x.Amount);

      if (Credit != Debit) throw new InvalidOperationException("Account " + account.Id + " is not balanced!");
    }
    // recursively checks the startAt account and all of its children
    private void CheckAccountsForCloseRecursive(Account startAt)
    {
      if (startAt.IsActive)
      {
        CheckAccountForClose(startAt);
      }

      foreach (var child in startAt.Children)
      {
        CheckAccountsForCloseRecursive(child);
      }
    }
    // sets isActive of account to false and marks the change for saving
    private void CloseAccountInternal(Account account)
    {
      account.IsActive = false;
      UnitOfWork.GetRepository<Account>().Update(account);
    }
    // recursively closing all children accounts before closing startAt
    private void CloseAccountsRecursive(Account startAt)
    {
      foreach (var child in startAt.Children)
      {
        CloseAccountsRecursive(child);
      }

      if (startAt.IsActive)
      {
        CloseAccountInternal(startAt);
      }
    }

    #endregion Helper methods
  }
}
