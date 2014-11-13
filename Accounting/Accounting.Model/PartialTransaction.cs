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
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public PartialTransactionType Type
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public Account Account
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }

    public Transaction Transaction
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
