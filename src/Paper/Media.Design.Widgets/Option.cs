using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public class Option
  {
    public Option()
    {
    }

    public Option(object value)
    {
      this.Value = value;
    }

    public Option(object value, string title)
    {
      this.Value = value;
      this.Title = title;
    }

    public object Value { get; set; }

    public string Title { get; set; }

    public override string ToString()
    {
      return (Title != null) ? $"{Value}" : $"{Value}:{Title}";
    }

    public static Option<T> Create<T>(T value)
    {
      return new Option<T>(value);
    }

    public static Option<T> Create<T>(T value, string title)
    {
      return new Option<T>(value, title);
    }
  }
}