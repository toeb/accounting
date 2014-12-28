using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class CloseAccountCommand
  {
    /// <summary>
    /// Set the account id of the account to be closed.<br/>
    /// The account is required to be balanced
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Specify, whether children accounts should also be closed.<br/>
    /// case true: the subtree is traversed and each active child is also checked and closed<br/>
    /// case false: the direct children (if any) are required to be closed, otherwise
    /// an exception is thrown and the command is aborted without change
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// This field is filled by the AccountingFacade after command execution
    /// </summary>
    public Account ClosedAccount { get; set; }
  }
}
