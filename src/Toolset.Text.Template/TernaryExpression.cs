using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class TernaryExpression : Expression
  {
    private Expression truePath;
    private Expression falsePath;

    public TernaryExpression(Expression truePath, Expression falsePath)
    {
      this.truePath = truePath ?? LiteralExpression.Null;
      this.falsePath = falsePath ?? LiteralExpression.Null;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      if (input.IsNone)
      {
        var result = falsePath.Evaluate(input, target, context);
        return result;
      }
      else
      {
        var result = truePath.Evaluate(input, target, context);
        return result;
      }
    }
  }
}
