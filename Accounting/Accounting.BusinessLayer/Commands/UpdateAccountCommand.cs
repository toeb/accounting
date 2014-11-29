using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public class UpdateAccountCommand
  {
    /// <summary>
    /// Id of the account, where an update is requested
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Optional new value for the account number. The number is required to be unique across all accounts
    /// </summary>
    public string NewNumber { get; set; }

    /// <summary>
    /// Optional new value for the account display shortname. The shortname is required to be unique across all accounts
    /// </summary>
    public string NewShortName { get; set; }

    /// <summary>
    /// Optional new value for the account display name. The name is required to be unique across all accounts
    /// </summary>
    public string NewName { get; set; }

    /// <summary>
    /// This field is set by the AccountingFacade after successful update of specified fields
    /// </summary>
    public Account ModifiedAccount { get; set; }
  }
}
