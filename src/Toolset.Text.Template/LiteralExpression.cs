using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class LiteralExpression : Expression
  {
    private string literal;

    public LiteralExpression(string literal)
    {
      this.literal = literal;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      return new Pipe(literal);
    }
  }
}
