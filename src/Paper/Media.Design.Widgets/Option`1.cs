using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public class Option<T> : Option
  {
    public Option()
    {
    }

    public Option(T value)
    {
      this.Value = value;
    }

    public Option(T value, string title)
    {
      this.Value = value;
      this.Title = title;
    }

    public new T Value
    {
      get => (base.Value is T) ? (T)base.Value : default(T);
      set => base.Value = value;
    }
  }
}