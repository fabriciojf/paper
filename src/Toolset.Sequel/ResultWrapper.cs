using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  internal class ResultWrapper<TTarget, TValue, TItem> : IResult<TItem>
    where TTarget : IResult<TValue>
  {
    private readonly TTarget target;
    private readonly Func<TValue, TItem> converter;

    public ResultWrapper(TTarget target, Func<TValue, TItem> converter)
    {
      this.target = target;
      this.converter = converter;
    }

    public event EventHandler Disposed
    {
      add => target.Disposed += value;
      remove => target.Disposed -= value;
    }
    public TItem Current => converter.Invoke(target.Current);
    public bool Read() => target.Read();
    public bool NextResult() => target.Read();
    public void Reset() => target.Reset();
    public void Cancel() => target.Cancel();
    public IResult<TItem> Clone() => new ResultWrapper<TTarget, TValue, TItem>(target, converter);
    public void Dispose() => target.Dispose();

    object IResult.Current => converter.Invoke(target.Current);
    object ICloneable.Clone() => new ResultWrapper<TTarget, TValue, TItem>(target, converter);

    public IEnumerator<TItem> GetEnumerator()
    {
      var items = target.Cast<TValue>().Select(converter);
      return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      var items = target.Cast<TValue>().Select(converter);
      return items.GetEnumerator();
    }
  }
}
