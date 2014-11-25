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
  public static class BillTransactionCommandExtensions
  {
  
    public static AddPartialTransactionCommand AddCreditor(this BillTransactionCommand cmd, decimal amount, int accountId)
    {
  
      var p = new AddPartialTransactionCommand()
      {
        Amount = amount,
        AccountId = accountId
      };
  
      cmd.Credits.Add(p);
      return p;
    }
  
    public static AddPartialTransactionCommand AddDebitor(this BillTransactionCommand cmd, decimal amount, int accountId)
    {
  
      var p = new AddPartialTransactionCommand()
      {
        Amount = amount,
        AccountId = accountId
      };
  
      cmd.Debits.Add(p);
      return p;
    }
  }
}
