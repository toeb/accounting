using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Accounting.BusinessLayer
{
  /// <summary>
  /// Ensure that a command is first validated, then handled
  /// </summary>
  /// <typeparam name="TCommand"></typeparam>
  class ValidatingHandlerDecorator<TCommand> : ICommandHandler<TCommand>
  {
    private ICommandHandler<TCommand> handler;

    public ValidatingHandlerDecorator(ICommandHandler<TCommand> handler)
    {
      this.handler = handler;
    }

    public void Handle(TCommand command)
    {
      IEnumerable<Exception> errors;
      if (handler.Validate(command, out errors))
      {
        handler.Handle(command);
      }
      else
      {
        if (errors.Count() > 1)
          throw new InvalidOperationException("Multiple validation errors occurred!", new AggregateException(errors));
        else if (errors.Count() == 1)
          throw errors.First();
      }
    }

    public bool Validate(TCommand command, out IEnumerable<Exception> ValidationErrors)
    {
      return handler.Validate(command, out ValidationErrors);
    }
  }
}
