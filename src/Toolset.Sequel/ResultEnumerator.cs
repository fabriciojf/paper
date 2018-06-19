using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Toolset.Sequel
{
  public class ResultEnumerator<T> : IEnumerator<T>
  {
    private readonly IResult<T> result;

    public ResultEnumerator(IResult<T> result)
    {
      this.result = result;
    }

    public T Current
    {
      get { return result.Current; }
    }

    object System.Collections.IEnumerator.Current
    {
      get { return result.Current; }
    }

    public bool MoveNext()
    {
      var ready = result.Read();
      if (!ready)
      {
        result.NextResult();
        ready = result.Read();
      }
      return ready;
    }

    public void Reset()
    {
      this.result.Reset();
    }

    public void Dispose()
    {
      result.Dispose();
    }
  }
}
