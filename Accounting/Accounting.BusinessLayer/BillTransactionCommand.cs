using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{

  public class BillTransactionCommand
  {
    public BillTransactionCommand() { Credits = new List<AddPartialTransactionCommand>(); Debits = new List<AddPartialTransactionCommand>(); }
    public string Receipt { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string TransactionText { get; set; }


    public IList<AddPartialTransactionCommand> Credits { get; set; }
    public IList<AddPartialTransactionCommand> Debits { get; set; }


    /// <summary>
    /// output transaction 
    /// </summary>
    public Transaction Transaction { get; set; }
  }
}
