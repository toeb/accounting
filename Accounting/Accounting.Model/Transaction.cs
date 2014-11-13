using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Model
{
  public class Transaction : Meta
  {
    public string ReceiptNumber
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public DateTime ReceiptDate
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public string Text
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public virtual System.Collections.Generic.IList<Accounting.Model.PartialTransaction> Partials
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public Transaction Storno
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }
  }
}
