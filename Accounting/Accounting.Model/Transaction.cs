using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Model
{
  public class Transaction : Meta
  {
    public Transaction()
    {
      Partials = new List<PartialTransaction>();
      ReceiptDate = DateTime.Now;
    }
    public string ReceiptNumber
    {
      get;
      set;
    }

    public DateTime ReceiptDate
    {
      get;
      set;
    }

    public string Text
    {
      get;
      set;
    }

    public virtual IList<PartialTransaction> Partials
    {
      get;
      set;
    }

    public Transaction Storno
    {
      get;
      set;
    }
  }
}
