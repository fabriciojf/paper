using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Paper.Media.Utils
{
  internal class Expressions
  {
    public static MemberExpression FindMemberExpression(Expression expression)
    {
      while (true)
      {
        if (expression is MemberExpression)
        {
          return (MemberExpression)expression;
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
          return null;
        }
      }
    }
  }
}