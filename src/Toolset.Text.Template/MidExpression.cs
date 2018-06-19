using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class MidExpression : Expression
  {
    private Expression startExpression;
    private Expression countExpression;

    public MidExpression(Expression start, Expression count)
    {
      this.startExpression = start ?? LiteralExpression.Null;
      this.countExpression = count ?? LiteralExpression.Null;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      var startInput = startExpression.Evaluate(input, target, context);
      var countInput = countExpression.Evaluate(input, target, context);

      var text = input.ValueAsString;
      var start = startInput.ValueAsInt;
      var count = countInput.ValueAsInt;

      var reverse = (start < 0);

      if (reverse)
      {
        start = -start;
        text = new string(text.Reverse().ToArray());
      }

      // corrigindo indice
      // o comando inicia em 1 mas o substring inicia em 0
      if (start > 0)
        start--;

      if (count == 0)
        count = int.MaxValue;

      var result = string.Empty;

      if (start < text.Length)
      {
        if (count > text.Length || (start + count) > text.Length)
          count = text.Length - start;

        result = text.Substring(start, count);
      }

      if (reverse)
        result = new string(result.Reverse().ToArray());

      return new Pipe(result);
    }
  }
}
