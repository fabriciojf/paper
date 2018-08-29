using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Paper.Media.Utilities
{
  internal class ExpressionUtils
  {
    public static MemberExpression FindMemberExpression(Expression expression)
    {
      return FindMemberExpressions(expression).FirstOrDefault();
    }

    public static IEnumerable<MemberExpression> FindMemberExpressions(Expression expression)
    {
      while (true)
      {
        if (expression is MemberExpression)
        {
          yield return (MemberExpression)expression;
        }

        if (expression is LambdaExpression)
        {
          expression = ((LambdaExpression)expression).Body;
        }
        else if (expression is UnaryExpression)
        {
          expression = ((UnaryExpression)expression).Operand;
        }
        else
        {
          yield break;
        }
      }
    }
  }
}