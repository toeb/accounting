using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class BillTransactionCommand
  {
    public BillTransactionCommand() { PartialTransactions = new List<PartialTransaction>(); }
    public string Receipt { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string TransactionText { get; set; }

    /// <summary>
    ///  partial transactions to perform
    /// </summary>
    public IList<PartialTransaction> PartialTransactions { get; set; }

    /// <summary>
    /// output transaction 
    /// </summary>
    public Transaction Transaction { get; set; }
  }
}
