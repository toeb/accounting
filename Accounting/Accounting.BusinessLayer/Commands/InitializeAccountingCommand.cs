using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  public enum InitializationActions
  {
    ExpectNewDatabase,
    DropInactive,
    DropOld,
    RepairOld
  }

  public class InitializeAccountingCommand
  {
    /// <summary>
    /// When the account initialization is called, there might be an existing system in
    /// the database. In this case, InitializationAction decides how to deal with the case.
    /// 
    /// ExpectNewDatabase: throws an exception if any accounts are detected in the system.
    /// 
    /// DropInactive: Throws an exception if any active account is detected in the system.
    /// Inactive accounts are allowed - they will be deleted before new initialization.
    /// 
    /// DropOld: Deletes any existing account, no matter whether it is inactive, active,
    /// balanced and so on.
    /// 
    /// RepairOld: Creates new accounts for any initial account which is not present.
    /// For existing initial accounts, it resets the settings (activating inactive, ...)
    /// </summary>
    public InitializationActions InitializationAction { get; set; }

    public IEnumerable<Account> CreatedOrModifiedAccounts { get; set; }
  }
}
