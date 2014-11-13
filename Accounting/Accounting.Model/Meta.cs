using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.Model
{
  public class Meta
  {

    public Meta()
    {
      CreationDate = DateTime.Now;
      LastModified = DateTime.Now;

    }
    public int Id
    {
      get;
      set;
    }

    public DateTime CreationDate
    {
      get;
      set;
    }

    public DateTime LastModified
    {
      get;
      set;
    }
  }
}
