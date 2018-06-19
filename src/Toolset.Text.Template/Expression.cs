using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  internal abstract class Expression
  {
    public static readonly Expression Null = new LiteralExpression(null);

    public string Compute(object target, object context = null)
    {
      var result = Evaluate(Pipe.None, target, (context ?? new { }));
      return (result.Value ?? "").ToString();
    }

    internal abstract Pipe Evaluate(Pipe input, object target, object context);
  }
}
