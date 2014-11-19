using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class RevertTransactionCommand
  {
    public int TransactionId { get; set; }

    public Transaction RevertedTransaction { get; set; }
  }
}
