using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Data;
using Toolset.Reflection;

namespace Toolset
{
  public class Values
  {
    public class Range
    {
      public Range(object min, object max)
      {
        this.Min = min;
        this.Max = max;
      }

      public object Min { get; }
      public object Max { get; }
    }

    public Values(object value)
    {
      this.RawValue = value;

      if (value.IsNull())
      {
        this.IsNull = true;
      }
      else if (value is string)
      {
        this.IsText = true;
        this.Text = (string)value;
      }
      else if (value is IEnumerable)
      {
        this.IsArray = true;
        this.Array = ((IEnumerable)value).Cast<object>();
      }
      else if (value.Has("Min") || value.Has("Max"))
      {
        this.Min = value.Get("Min");
        this.Max = value.Get("Max");
        this.IsNull = (this.Min.IsNull() && this.Max.IsNull());
        this.IsRange = !this.IsNull;
      }
      else
      {
        this.IsValue = true;
        this.Value = value;
      }
    }

    public Values(object min, object max)
      : this((min ?? max) != null ? (object)new Range(min, max) : null)
    {
    }

    public bool IsNull { get; }

    public bool IsValue { get; }

    public bool IsText { get; }

    public bool IsArray { get; }

    public bool IsRange { get; }

    public bool HasWildcard
      => IsText && (Text?.Contains("%") == true || Text?.Contains("_") == true);

    public object RawValue { get; }

    public object Value { get; }

    public string Text { get; }

    public string TextPattern
      => (Text != null)
        ? $"^{Regex.Escape(Text).Replace("%", ".*").Replace("_", ".")}$"
        : null;

    public object Min { get; }

    public object Max { get; }

    public IEnumerable<object> Array { get; }

    public override int GetHashCode()
      => (RawValue ?? this).GetHashCode();

    public override bool Equals(object obj)
      => (RawValue ?? this).Equals(obj is Values ? ((Values)obj).RawValue : obj);

    public override string ToString()
      => RawValue?.ToString();

    public static Values<T> Create<T>(T value)
      => new Values<T>(value);

    public static Values Create(object value)
      => new Values(value);
  }
}