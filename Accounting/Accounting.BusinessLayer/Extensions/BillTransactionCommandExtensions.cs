using Accounting.BusinessLayer;

namespace Accounting.BusinessLayer
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
