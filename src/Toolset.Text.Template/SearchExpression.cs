using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Text.Template
{
  class SearchExpression : Expression
  {
    private Expression regexExpression;
    private Expression replacementExpression;

    public SearchExpression(Expression regex)
    {
      this.regexExpression = regex;
      this.replacementExpression = null;
    }

    public SearchExpression(Expression regex, Expression replacement)
    {
      this.regexExpression = regex;
      this.replacementExpression = replacement;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      if (replacementExpression == null)
        return SearchAndReport(input, target, context);
      else
        return SearchAndReplace(input, target, context);
    }

    private Pipe SearchAndReport(Pipe input, object target, object context)
    {
      var regex = regexExpression.Evaluate(input, target, context);
      var result = Regex.IsMatch(input.ValueAsString, regex.ValueAsString);
      return new Pipe(result);
    }

    private Pipe SearchAndReplace(Pipe input, object target, object context)
    {
      var regex = regexExpression.Evaluate(input, target, context);
      var replacement = replacementExpression.Evaluate(input, target, context);
      var result = Regex.Replace(input.ValueAsString, regex.ValueAsString, replacement.ValueAsString);
      return new Pipe(result);
    }
  }
}
