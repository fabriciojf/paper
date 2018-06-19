using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class Result<T> : IResult<T>
  {
    public static readonly Result<T> Empty = new Result<T>(() => null, reader => default(T));

    public event EventHandler Disposed;

    private readonly Func<DbCommand> factory;
    private readonly Func<DbDataReader, T> transform;

    private DbCommand command;
    private DbDataReader reader;
    
    public Result(Func<DbCommand> factory, Func<DbDataReader, T> transform)
    {
      this.factory = factory;
      this.transform = transform;
      Reset();
    }

    public T Current
    {
      get;
      private set;
    }

    object IResult.Current
    {
      get { return this.Current; }
    }

    protected DbCommand Command { get { return command; } }
    protected DbDataReader Reader { get { return reader; } }

    public bool Read()
    {
      if (reader == null)
        return false;

      var ready = reader.Read();
      this.Current = ready ? transform.Invoke(reader) : default(T);
      return ready;
    }

    public bool NextResult()
    {
      if (reader == null)
        return false;

      return reader.NextResult();
    }

    public void Reset()
    {
      if (this.reader != null)
      {
        this.reader.Dispose();
        this.reader = null;
      }
      if (this.command != null)
      {
        this.command.Dispose();
        this.command = null;
      }

      this.command = this.factory.Invoke();
      if (this.command != null)
      {
        this.reader = this.command.ExecuteReader();
      }
    }

    public void Cancel()
    {
      if (this.command != null)
      {
        this.command.Cancel();
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      var enumerator = new ResultEnumerator<T>(this);
      return enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public void Dispose()
    {
      if (this.reader != null)
      {
        this.reader.Dispose();
        this.reader = null;
      }
      if (this.command != null)
      {
        this.command.Dispose();
        this.command = null;
      }

      OnDisposed(EventArgs.Empty);
    }

    protected virtual void OnDisposed(EventArgs args)
    {
      if (Disposed != null)
      {
        Disposed.Invoke(this, args);
      }
    }

    public IResult<T> Clone()
    {
      var clone = new Result<T>(this.factory, this.transform);
      return clone;
    }

    object ICloneable.Clone()
    {
      return this;
    }
  }
}
