using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class OpenAccountCommand
  {
    public string AccountName { get; set; }
    public string AccountShortname { get; set; }
    public string AccountNumber { get; set; }

    public int? ParentAccountId { get; set; }


    public Accounting.Model.Account Account
    {
      get;
      set;
    }

    public int CategoryId { get; set; }
  }
}
