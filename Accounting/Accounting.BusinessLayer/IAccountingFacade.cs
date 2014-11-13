using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public interface IAccountingFacade
  {
    void OpenAccount(OpenAccountCommand command);


  }

}
