using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;
using Toolset.Data;
using Toolset.Reflection;

namespace Toolset
{
  public class Any
  {
    public Any()
      : this(null, x => x)
    {
    }

    public Any(object value)
      : this(value, x => x)
    {
    }

    public Any(object min, object max)
      : this(new { min, max }, x => x)
    { 
    }

    protected Any(object value, Func<object, object> caster)
    {
      while (value is Any)
      {
        value = ((Any)value).Value;
      }

      this.Value = value;

      if (value.IsNull())
      {
        return;
      }

      if (value is string)
      {
        Text = (string)value;
        TextPattern =
          (Text.Contains("%") || Text.Contains("_"))
            ? CreateTextPattern(Text)
            : null;
      }
      else if (value is IEnumerable)
      {
        List = ((IEnumerable)value).Cast<object>().Select(caster).ToArray();
      }
      else if (value._Has("Min") || value._Has("Max"))
      {
        var min = caster.Invoke(value._Get("Min"));
        var max = caster.Invoke(value._Get("Max"));
        Range = new Range(min, max);
      }
      else if (!value.IsNull())
      {
        Raw = caster.Invoke(value);
      }
    }

    public bool IsNull => Value.IsNull();

    public bool IsText => Text != null;

    public bool IsList => List != null;

    public bool IsRange => Range != null;

    public bool IsRaw => Raw != null;

    public object Value { get; }

    public string Text { get; }

    public string TextPattern { get; }

    public bool TextHasWildcard => TextPattern != null;

    public IEnumerable<object> List { get; }

    public Range Range { get; }

    public object Raw { get; }

    public override string ToString()
    {
      if (IsList)
        return string.Join(", ", List);

      if (IsRange)
        return Range.ToString();

      return Value?.ToString();
    }

    public static Type GetUnderlyingType(Type type)
    {
      if (!typeof(Any).IsAssignableFrom(type))
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
  }
}