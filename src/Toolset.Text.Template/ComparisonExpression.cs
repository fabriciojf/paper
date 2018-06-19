using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Text.Template
{
  class ComparisonExpression : Expression
  {
    public static readonly string[] KnownOperations = { "%eq", "%ne", "%gt", "%ge", "%lt", "%le", "%is", "%in" };

    private string operation;
    private Func<Pipe, Pipe, bool> function;
    private Expression comparator;
    private Expression truePath;
    private Expression falsePath;

    public ComparisonExpression(string operation, Expression comparator, Expression truePath, Expression falsePath)
    {
      this.operation = operation;
      this.comparator = comparator ?? LiteralExpression.Null;
      this.truePath = truePath ?? LiteralExpression.Null;
      this.falsePath = falsePath ?? LiteralExpression.Null;

      var methodName = "Compare" + char.ToUpper(operation.Skip(1).First()) + operation.Skip(2).First();
      var method = GetType().GetMethod(methodName);
      this.function = new Func<Pipe, Pipe, bool>(
        (a, b) =>
        {
          var args = new object[] { a, b };
          var result = method.Invoke(this, args);
          return (bool)result;
        }
      );
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      try
      {
        var compareValue = this.comparator.Evaluate(input, target, context);
        var truth = this.function.Invoke(input, compareValue);
        if (truth)
        {
          var result = truePath.Evaluate(input, target, context);
          return result;
        }
        else
        {
          var result = falsePath.Evaluate(input, target, context);
          return result;
        }
      }
      catch (Exception ex)
      {
        ex.Report("Não foi possível aplicar o operador: " + operation);
        return Pipe.None;
      }
    }

    public bool CompareEq(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      if (a.Equals(b))
        return true;
      
      var type = a.Value.GetType();
      var terms = b.Array ?? new[] { b.Value };

      var comparand = (IComparable)a.Value;
      var comparators =
        from term in terms
        let convertedTerm = CastExpression.ConvertValueTo(term, type)
        select (IComparable)convertedTerm;

      var ok = comparators.Any(comparator => comparand.CompareTo(comparator) == 0);
      return ok;
    }

    public bool CompareNe(Pipe a, Pipe b)
    {
      return !CompareEq(a, b);
    }

    public bool CompareGt(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      var type = a.Value.GetType();
      var terms = b.Array ?? new[] { b.Value };

      var comparand = (IComparable)a.Value;
      var comparators =
        from term in terms
        let convertedTerm = CastExpression.ConvertValueTo(term, type)
        select (IComparable)convertedTerm;

      var ok = comparators.All(comparator => comparand.CompareTo(comparator) > 0);
      return ok;
    }

    public bool CompareGe(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      var type = a.Value.GetType();
      var terms = b.Array ?? new[] { b.Value };

      var comparand = (IComparable)a.Value;
      var comparators =
        from term in terms
        let convertedTerm = CastExpression.ConvertValueTo(term, type)
        select (IComparable)convertedTerm;

      var ok = comparators.All(comparator => comparand.CompareTo(comparator) >= 0);
      return ok;
    }

    public bool CompareLt(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      var type = a.Value.GetType();
      var terms = b.Array ?? new[] { b.Value };

      var comparand = (IComparable)a.Value;
      var comparators =
        from term in terms
        let convertedTerm = CastExpression.ConvertValueTo(term, type)
        select (IComparable)convertedTerm;

      var ok = comparators.All(comparator => comparand.CompareTo(comparator) < 0);
      return ok;
    }

    public bool CompareLe(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      var type = a.Value.GetType();
      var terms = b.Array ?? new[] { b.Value };

      var comparand = (IComparable)a.Value;
      var comparators =
        from term in terms
        let convertedTerm = CastExpression.ConvertValueTo(term, type)
        select (IComparable)convertedTerm;

      var ok = comparators.All(comparator => comparand.CompareTo(comparator) <= 0);
      return ok;
    }

    public bool CompareIs(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      var terms = b.Array ?? new[] { b.Value };

      var comparand = (a.Value ?? "").ToString();
      var comparators =
        from term in terms
        let text = (term ?? "").ToString()
        select CreateRegex(text);

      var ok = comparators.Any(regex => regex.IsMatch(comparand));
      return ok;
    }

    public bool CompareIn(Pipe a, Pipe b)
    {
      if (a.IsNone)
        return b.IsNone;

      var array = b.Array ?? new object[] { b.Value, b.Value };

      var minRaw =(IComparable)array.FirstOrDefault();
      var maxRaw = (IComparable)array.Skip(1).LastOrDefault(); 

      IComparable operand = ((IComparable)a.Value);
      IComparable minValue = (IComparable)CastExpression.ConvertValueTo(minRaw, operand.GetType());
      IComparable maxValue = (IComparable)CastExpression.ConvertValueTo(maxRaw, operand.GetType());

      if (operand.CompareTo(minValue) < 0)
        return false;

      if (operand.CompareTo(maxValue) > 0)
        return false;

      return true;
    }

    private Regex CreateRegex(string text)
    {
      text = text.Replace("*", "§");
      text = Regex.Escape(text);
      text = "^" + text.Replace("§", ".*") + "$";
      return new Regex(text);
    }
  }
}
