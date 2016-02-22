using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer.CommandHandlers
{
  class RevertTransactionCommandHandler : ICommandHandler<RevertTransactionCommand>
  {
    private IUnitOfWork UnitOfWork;

    public RevertTransactionCommandHandler(IUnitOfWork UnitOfWork)
    {
      this.UnitOfWork = UnitOfWork;
    }

    public void Handle(RevertTransactionCommand command)
    {
      var transactions = UnitOfWork.GetRepository<Transaction>();

      var transactionToRevert = transactions.GetByID(command.TransactionId);

      var revertedTransaction = new Transaction()
      {
        Storno = transactionToRevert,
        ReceiptDate = transactionToRevert.ReceiptDate,
        ReceiptNumber = transactionToRevert.ReceiptNumber,
        Text = string.IsNullOrWhiteSpace(command.Text)
            ? "Storno: " + transactionToRevert.Text
            : command.Text,
        Partials = transactionToRevert.Partials.Select(p => new PartialTransaction() { Amount = -p.Amount, Type = p.Type }).ToList()
      };

      transactionToRevert.Storno = revertedTransaction;

      transactions.Create(revertedTransaction);
      transactions.Update(transactionToRevert);
      UnitOfWork.Save();

      transactions.Refresh(revertedTransaction.Storno);
      transactions.Refresh(revertedTransaction);

      command.RevertedTransaction = revertedTransaction;
    }

    public bool Validate(RevertTransactionCommand command, out IEnumerable<Exception> ValidationErrors)
    {
      var errors = new List<Exception>();
      ValidationErrors = errors;

      if (command.TransactionId <= 0)
      {
        errors.Add(new InvalidOperationException("TransactionId greater 0 is required"));
      }
      else if (UnitOfWork.GetRepository<Transaction>().GetByID(command.TransactionId) == null)
      {
        var transactionToRevert = UnitOfWork.GetRepository<Transaction>().GetByID(command.TransactionId);
        if (transactionToRevert == null)
        {
          errors.Add(new InvalidOperationException("the transaction to revert does not exist"));
        }
        else if (transactionToRevert.Storno != null)
        {
          errors.Add(new InvalidOperationException("the transaction was already reverted"));
        }
      }

      return errors.Count == 0;
    }
  }
}
