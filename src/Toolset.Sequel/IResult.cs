using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Toolset.Sequel
{
  public interface IResult : IDisposable, IEnumerable, ICloneable
  {
    event EventHandler Disposed;

    object Current { get; }

    bool Read();

    bool NextResult();

    void Reset();

    void Cancel();
  }

  public interface IResult<T> : IResult, IEnumerable<T>
  {
    new T Current { get; }

    new IResult<T> Clone();
  }
}
