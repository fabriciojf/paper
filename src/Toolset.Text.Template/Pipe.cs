using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Text.Template
{
  struct Pipe
  {
    public static Pipe None = new Pipe();

    public object Value;
    public object[] Array;

    public Pipe(object value)
    {
      this.Value = (value is Pipe) ? ((Pipe)value).Value : value;
      this.Array = null;
    }

    public Pipe(object value, object[] array)
    {
      this.Value = (value is Pipe) ? ((Pipe)value).Value : value;
      this.Array = array;
    }

    public string ValueAsString
    {
      get { return (Value ?? "").ToString(); }
    }

    public int ValueAsInt
    {
      get
      {
        if (Value == null) return 0;
        if (Value is int) return (int)Value;
        if (Value is float) return (int)(float)Value;
        if (Value is double) return (int)(double)Value;
        if (Value is bool) return ((bool)Value) ? 1 : 0;

        var text = (Value ?? "").ToString();
        int number;
        return int.TryParse(text, out number) ? number : 0;
      }
    }

    public bool IsNone
    {
      get
      {
        if (Value == null) return true;
        if (Value is string) return string.IsNullOrEmpty(Value as string);
        if (Value is bool) return false.Equals(Value);
        if (Value is int) return 0.Equals(Value);
        if (Value is long) return 0L.Equals(Value);
        if (Value is float) return 0F.Equals(Value);
        if (Value is double) return 0D.Equals(Value);
        return false;
      }
    }

    public static implicit operator string(Pipe pipe)
    {
      return (pipe.Value == null) ? null : pipe.Value.ToString();
    }

    public static implicit operator Pipe(string text)
    {
      return new Pipe { Value = text };
    }

    #region Overridings

    public override string ToString()
    {
      if (Value == null) return base.ToString();
      return Value.ToString();
    }

    public override bool Equals(object obj)
    {
      if (Value == null) return false;
      return Value.Equals(obj);
    }

    public override int GetHashCode()
    {
      if (Value == null) return base.GetHashCode();
      return Value.GetHashCode();
    }

    #endregion
  }
}
