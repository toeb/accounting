using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class ListAccountsCommand
  {
    public IQueryable<Account> Query { get; set; }
  }
}
