using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.DataLayer
{
  public class AccountingDbContext : DbContext
  {
    public AccountingDbContext() { }
    public AccountingDbContext(string connectionstring) : base(connectionstring) { }
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
    }


    public IDbSet<Transaction> Transactions { get; set; }
   // public IDbSet<Meta> Transactions { get; set; }
    public IDbSet<Account> Accounts { get; set; }
    public IDbSet<PartialTransaction> PartialTransactions { get; set; }
    public IDbSet<AccountCategory> AccountCategories { get; set; }
  }


}
