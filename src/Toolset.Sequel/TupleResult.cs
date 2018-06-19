using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class TupleResult<T1> : Result<Tuple<T1>>
  {
    new public static readonly
      TupleResult<T1> Empty
      = new TupleResult<T1>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }

    internal static Tuple<T1> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      );
    }
  }

  public class TupleResult<T1, T2> : Result<Tuple<T1, T2>>
  {
    public new static readonly
      TupleResult<T1, T2> Empty
      = new TupleResult<T1, T2>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }
    public T2 Item2 { get { return Current.Item2; } }

    internal static Tuple<T1, T2> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      , reader.Get<T2>(1)
      );
    }
  }

  public class TupleResult<T1, T2, T3> : Result<Tuple<T1, T2, T3>>
  {
    public new static readonly
      TupleResult<T1, T2, T3> Empty
      = new TupleResult<T1, T2, T3>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }
    public T2 Item2 { get { return Current.Item2; } }
    public T3 Item3 { get { return Current.Item3; } }

    internal static Tuple<T1, T2, T3> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      , reader.Get<T2>(1)
      , reader.Get<T3>(2)
      );
    }
  }

  public class TupleResult<T1, T2, T3, T4> : Result<Tuple<T1, T2, T3, T4>>
  {
    public new static readonly
      TupleResult<T1, T2, T3, T4> Empty
      = new TupleResult<T1, T2, T3, T4>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }
    public T2 Item2 { get { return Current.Item2; } }
    public T3 Item3 { get { return Current.Item3; } }
    public T4 Item4 { get { return Current.Item4; } }

    internal static Tuple<T1, T2, T3, T4> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      , reader.Get<T2>(1)
      , reader.Get<T3>(2)
      , reader.Get<T4>(3)
      );
    }
  }

  public class TupleResult<T1, T2, T3, T4, T5> : Result<Tuple<T1, T2, T3, T4, T5>>
  {
    public new static readonly
      TupleResult<T1, T2, T3, T4, T5> Empty
      = new TupleResult<T1, T2, T3, T4, T5>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }
    public T2 Item2 { get { return Current.Item2; } }
    public T3 Item3 { get { return Current.Item3; } }
    public T4 Item4 { get { return Current.Item4; } }
    public T5 Item5 { get { return Current.Item5; } }

    internal static Tuple<T1, T2, T3, T4, T5> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      , reader.Get<T2>(1)
      , reader.Get<T3>(2)
      , reader.Get<T4>(3)
      , reader.Get<T5>(4)
      );
    }
  }

  public class TupleResult<T1, T2, T3, T4, T5, T6> : Result<Tuple<T1, T2, T3, T4, T5, T6>>
  {
    public new static readonly
      TupleResult<T1, T2, T3, T4, T5, T6> Empty
      = new TupleResult<T1, T2, T3, T4, T5, T6>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }
    public T2 Item2 { get { return Current.Item2; } }
    public T3 Item3 { get { return Current.Item3; } }
    public T4 Item4 { get { return Current.Item4; } }
    public T5 Item5 { get { return Current.Item5; } }
    public T6 Item6 { get { return Current.Item6; } }

    internal static Tuple<T1, T2, T3, T4, T5, T6> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      , reader.Get<T2>(1)
      , reader.Get<T3>(2)
      , reader.Get<T4>(3)
      , reader.Get<T5>(4)
      , reader.Get<T6>(5)
      );
    }
  }

  public class TupleResult<T1, T2, T3, T4, T5, T6, T7> : Result<Tuple<T1, T2, T3, T4, T5, T6, T7>>
  {
    public new static readonly
      TupleResult<T1, T2, T3, T4, T5, T6, T7> Empty
      = new TupleResult<T1, T2, T3, T4, T5, T6, T7>(() => null);

    public TupleResult(Func<DbCommand> factory)
      : base(factory, CreateTuple)
    {
    }

    public T1 Item1 { get { return Current.Item1; } }
    public T2 Item2 { get { return Current.Item2; } }
    public T3 Item3 { get { return Current.Item3; } }
    public T4 Item4 { get { return Current.Item4; } }
    public T5 Item5 { get { return Current.Item5; } }
    public T6 Item6 { get { return Current.Item6; } }
    public T7 Item7 { get { return Current.Item7; } }

    internal static Tuple<T1, T2, T3, T4, T5, T6, T7> CreateTuple(DbDataReader reader)
    {
      return Tuple.Create(
        reader.Get<T1>(0)
      , reader.Get<T2>(1)
      , reader.Get<T3>(2)
      , reader.Get<T4>(3)
      , reader.Get<T5>(4)
      , reader.Get<T6>(5)
      , reader.Get<T7>(6)
      );
    }
  }
}
