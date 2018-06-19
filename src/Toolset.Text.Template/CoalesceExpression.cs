using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class CoalesceExpression : Expression
  {
    private readonly List<Expression> expressions = new List<Expression>();

    public void Add(Expression expression)
    {
      this.expressions.Add(expression);
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      var outputs = new Pipe[expressions.Count];
      
      var arguments = input.Array ?? new object[] { input };
      for (int i = 0; i < outputs.Length; i++)
      {
        var expression = expressions[i];

        var argument = (arguments.Length > i) ? arguments[i] : arguments.Last();
        var argumentValue = (argument is Pipe) ? ((Pipe)argument).Value : argument;
        var argumentPipe = new Pipe(argumentValue, input.Array);

        var output = expression.Evaluate(argumentPipe, target, context);
        outputs[i] = output;
      }

      var coalesce = (
        from x in outputs
        where !x.IsNone
        select x.Value
      ).FirstOrDefault();

      var array = (
        from x in outputs
        select x.Value
      ).ToArray();

      return new Pipe(coalesce, array);
    }
  }
}
