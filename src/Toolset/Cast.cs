using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset
{
  public static class Cast
  {
    public static object To(object value, Type targetType)
    {
      var sourceType = value?.GetType();

      if (value == null)
        return null;

      if (targetType.IsAssignableFrom(sourceType))
        return value;

      if (targetType == typeof(string))
        return value.ToString();

      if (targetType == typeof(DateTime) && value is string)
      {
        var text = (string)value;
        if (Regex.IsMatch(text, @"\d{4}-\d{2}-\d{2}.*"))
        {
          return DateTime.Parse(text);
        }
      }

      if (targetType == typeof(TimeSpan) && value is string)
      {
        var text = (string)value;
        if (Regex.IsMatch(text, @"(\d\.)?\d{2}:\d{2}.*"))
        {
          return DateTime.Parse(text);
        }
      }

      var flags = BindingFlags.Static | BindingFlags.Public;

      var methods = sourceType.GetMethods(flags).Concat(targetType.GetMethods(flags));
      var casting = (
        from method in methods
        where method.Name == "op_Implicit"
           || method.Name == "op_Explicit"
        where method.GetParameters().Length == 1
           && sourceType.IsAssignableFrom(method.GetParameters().Single().ParameterType)
           && targetType.IsAssignableFrom(method.ReturnType)
        select method
      ).FirstOrDefault();
      
      if (casting != null)
      {
        var castValue = casting.Invoke(null, new[] { value });
        return castValue;
      }

      targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

      var convertedValue = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);

      return convertedValue;
    }

    public static T To<T>(object value)
    {
      var convertedValue = To(value, typeof(T));
      return (convertedValue == null) ? default(T) : (T)convertedValue;
    }
  }
}