using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Data
{
  public static class Var
  {
    public static Type GetUnderlyingType(Type type)
    {
      if (!typeof(IVar).IsAssignableFrom(type))
        return null;

      var classType = type.GetGenericArguments().FirstOrDefault();
      return classType ?? typeof(object);
    }

    public static string CreateTextPattern(string text)
    {
      return (text != null)
        ? $"^{Regex.Escape(text).Replace("%", ".*").Replace("_", ".")}$"
        : "";
    }

    public static bool HasWildcards(string text)
    {
      return text?.Contains("%") == true || text?.Contains("_") == true;
    }
  }
}
