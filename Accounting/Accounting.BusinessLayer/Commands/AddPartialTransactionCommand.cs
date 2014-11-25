using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class AddPartialTransactionCommand
  {
    public decimal Amount { get; set; }
    public int AccountId { get; set; }
  
  
  }
}
