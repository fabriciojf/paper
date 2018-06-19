using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  class ReplaceExpression : Expression
  {
    private Expression criteriaExpression;
    private Expression replacementExpression;

    public ReplaceExpression(Expression criteria, Expression replacement)
    {
      this.criteriaExpression = criteria;
      this.replacementExpression = replacement ?? Expression.Null;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      var criteria = criteriaExpression.Evaluate(input, target, context);
      var replacement = replacementExpression.Evaluate(input, target, context);
      var result = input.ValueAsString.Replace(criteria.ValueAsString, replacement.ValueAsString);
      return new Pipe(result);
    }
  }
}
