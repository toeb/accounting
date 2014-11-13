using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Model
{
  public class PartialTransaction : Meta
  {

    public decimal Amount
    {
      get;
      set;
    }

    public PartialTransactionType Type
    {
      get;
      set;
    }

    public Account Account
    {
      get;
      set;
    }

    public Transaction Transaction
    {
      get;
      set;
    }
  }
}
