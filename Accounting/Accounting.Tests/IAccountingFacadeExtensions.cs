using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accounting.BusinessLayer;
using Accounting.DataLayer;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using Accounting.Model;
using System.Linq;

namespace Accounting.Tests
{
  /// <summary>
  /// mixin for IAccountingFacade
  /// </summary>
  public static class IAccountingFacadeExtensions
  {
    public static BillTransactionCommand BillTransaction(this IAccountingFacade self, Account debitor, Account creditor, decimal amount)
    {
      var command = new BillTransactionCommand();
  
      command.Receipt = "Manual Transaction";
      command.ReceiptDate = DateTime.Now;
      command.TransactionText = "Manual Transaction";
      command.PartialTransactions.Add(new PartialTransaction()
      {
        Account = debitor,
        Amount = amount,
        Type = PartialTransactionType.Debit
      });
      command.PartialTransactions.Add(new PartialTransaction()
      {
        Account = creditor,
        Amount = amount,
        Type = PartialTransactionType.Credit
      });
  
      self.BillTransaction(command);
      return command;
    }
    public static OpenAccountCommand OpenAccount(this IAccountingFacade self, string accountName,string accountNumber)
    {
      var command = new OpenAccountCommand();
      command.AccountName = accountName;
      command.AccountNumber = accountNumber;
      self.OpenAccount(command);
      return command;
    }
  }
}
