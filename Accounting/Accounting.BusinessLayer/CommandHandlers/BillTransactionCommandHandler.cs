using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer.CommandHandlers
{
  class BillTransactionCommandHandler : ICommandHandler<BillTransactionCommand>
  {
    private IUnitOfWork UnitOfWork;

    public BillTransactionCommandHandler(IUnitOfWork UnitOfWork)
    {
      this.UnitOfWork = UnitOfWork;
    }

    public void Handle(BillTransactionCommand command)
    {
      // get repository
      var Accounts = UnitOfWork.GetRepository<Account>();
      var Transactions = UnitOfWork.GetRepository<Transaction>();

      Trace.TraceInformation("Billing a transaction");

      var transactions = CreatePartialTransactions(Accounts, command.Credits, PartialTransactionType.Credit)
        .Concat(CreatePartialTransactions(Accounts, command.Debits, PartialTransactionType.Debit)).ToList();

      var transaction = new Transaction()
      {
        ReceiptDate = command.ReceiptDate.Value,
        ReceiptNumber = command.Receipt,
        Text = command.TransactionText,
        Storno = null,
        CreationDate = DateTime.Now,
        LastModified = DateTime.Now,
        Partials = transactions
      };

      Transactions.Create(transaction);
      UnitOfWork.Save();

      Trace.TraceInformation("Transaction was added to database");

      command.Transaction = transaction;
    }

    public bool Validate(BillTransactionCommand command, out IEnumerable<Exception> ValidationErrors)
    {
      var errors = new List<Exception>();
      ValidationErrors = errors;

      if (command == null)
      {
        errors.Add(new ArgumentNullException("command"));
      }
      if (string.IsNullOrWhiteSpace(command.TransactionText))
      {
        errors.Add(new InvalidOperationException("transaction does not have a text"));
      }
      if (string.IsNullOrWhiteSpace(command.Receipt))
      {
        errors.Add(new InvalidOperationException("transaction does not have a receipt"));
      }
      if (!command.ReceiptDate.HasValue)
      {
        errors.Add(new InvalidOperationException("transaction needs a valid receipt date"));
      }

      if (command.Credits == null)
      {
        errors.Add(new InvalidOperationException("credits may not be null"));
      }
      if (command.Debits == null)
      {
        errors.Add(new InvalidOperationException("debits may not be null"));
      }
      if (command.Credits.Any(p => p.Amount <= 0.0m))
      {
        errors.Add(new InvalidOperationException("partial transactions may not have an amount of 0 or less"));
      }
      if (command.Debits.Any(p => p.Amount <= 0.0m))
      {
        errors.Add(new InvalidOperationException("partial transactions may not have an amount of 0 or less"));
      }

      var creditSum = command.Credits.Aggregate(0.0m, (lhs, p) => lhs + p.Amount);
      var debitSum = command.Debits.Aggregate(0.0m, (lhs, p) => lhs + p.Amount);

      if (creditSum != debitSum)
      {
        errors.Add(new InvalidOperationException("transaction is not balanced"));
      }
      
      var Accounts = UnitOfWork.GetRepository<Account>();
      foreach (var accId in command.Credits.Concat(command.Debits).Select(x => x.AccountId).Distinct())
      {
        var acc = Accounts.GetByID(accId);
        if (acc == null)
        {
          errors.Add(new InvalidOperationException("partial transaction needs to have an existing account"));
        }
        else if (!acc.IsActive)
        {
          errors.Add(new InvalidOperationException("transaction may touch only active accounts"));
        }
      }

      return errors.Count == 0;
    }


    #region Helper methods

    private static IEnumerable<PartialTransaction> CreatePartialTransactions(
      IRepository<Account> Accounts,
      IEnumerable<AddPartialTransactionCommand> partials,
      PartialTransactionType type)
    {
      foreach (var partial in partials)
      {
        var obj = new PartialTransaction()
        {
          Type = type,
          Amount = partial.Amount,
          Account = Accounts.GetByID(partial.AccountId)
        };

        yield return obj;
      }
    }

    #endregion Helper methods
  }
}
