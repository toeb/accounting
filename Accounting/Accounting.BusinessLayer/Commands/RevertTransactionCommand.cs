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
    /// <summary>
    /// alternative text
    /// </summary>
    public string Text { get; set; }
    public Transaction RevertedTransaction { get; set; }
  }
}
