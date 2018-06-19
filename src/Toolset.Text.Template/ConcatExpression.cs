using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class ConcatExpression : Expression
  {
    private readonly List<Expression> expressions = new List<Expression>();

    public void Add(Expression expression)
    {
      this.expressions.Add(expression);
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      var builder = new StringBuilder();
      foreach (var expression in this.expressions)
      {
        var output = expression.Evaluate(input, target, context);
        builder.Append(output);
      }
      return builder.ToString();
    }
  }
}
