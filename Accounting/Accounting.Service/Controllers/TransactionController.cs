using Accounting.BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Accounting.Service.Controllers
{
  [Export(typeof(TransactionController))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class TransactionController : ApiController
  {

    [ImportingConstructor]
    public TransactionController([Import] IAccountingFacade accounting)
    {
      this.Accounting = accounting;
    }





    public IAccountingFacade Accounting { get; set; }
  }
}
