using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class Pipeline : Expression
  {
    private readonly List<Expression> expressions = new List<Expression>();

    public void Add(Expression expression)
    {
      this.expressions.Add(expression);
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      foreach (var expression in this.expressions)
      {
        input = expression.Evaluate(input, target, context);
      }
      return input;
    }
  }
}
