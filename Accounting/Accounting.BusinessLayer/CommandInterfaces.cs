using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BusinessLayer
{
  public interface ICommandValidator<TCommand>
  {
    bool Validate(TCommand command, out IEnumerable<Exception> ValidationErrors);
  }
  public interface ICommandHandler<TCommand> : ICommandValidator<TCommand>
  {
    void Handle(TCommand command);
  }
}
