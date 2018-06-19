using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design.Widgets
{
  public class Options<T> : Collection<Option<T>>
  {
    public Options()
    {
    }

    public Options(IEnumerable<Option<T>> items)
      : base(items)
    {
    }

    public Options(params Option<T>[] items)
      : base(items)
    {
    }

    public virtual void Add(T value)
    {
      this.Add(Option.Create(value));
    }

    public virtual void Add(T value, string title)
    {
      this.Add(Option.Create(value, title));
    }

    public virtual void AddMany(IEnumerable<T> items)
    {
      var options = items.Select(Option.Create);
      this.AddMany(options);
    }

    #region Conversões implícitas

    public static implicit operator Option<T>[] (Options<T> items) => items.ToArray();

    public static implicit operator List<Option<T>>(Options<T> items) => new List<Option<T>>(items);

    public static implicit operator Options<T>(Option<T>[] items) => new Options<T>(items);

    public static implicit operator Options<T>(List<Option<T>> items) => new Options<T>(items);

    #endregion

  }
}