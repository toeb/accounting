using Accounting.BusinessLayer;
using Accounting.Model;
using System;

namespace Accounting.BusinessLayer
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
      command.AddDebitor(amount, debitor.Id);
      command.AddCreditor(amount, creditor.Id);
  
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
