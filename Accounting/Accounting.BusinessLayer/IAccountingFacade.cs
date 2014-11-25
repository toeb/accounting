using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public interface IAccountingFacade
  {
    /// <summary>
    /// Opens an account / performs validation on the account which is to be opened
    /// throws Invalidoperatioexception if the commands values are not valid
    /// </summary>
    /// <param name="command"></param>
    void OpenAccount(OpenAccountCommand command);


    void BillTransaction(BillTransactionCommand command);

  }



}
