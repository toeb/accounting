﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Model
{
  public class Account : Meta
  {
    public Account()
    {
      Children = new List<Account>();
    }

    public bool IsActive { get; set; }
    public string Name
    {
      get;
      set;
    }

    public string Number
    {
      get;
      set;
    }

    public string ShortName
    {
      get;
      set;
    }

    public virtual Account Parent
    {
      get;
      set;
    }

    public virtual IList<Account> Children
    {
      get;
      set;
    }

    public AccountCategory AccountCategory
    {
      get;
      set;
    }
  }
}
