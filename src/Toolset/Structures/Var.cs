using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Toolset.Collections;

namespace Toolset.Structures
{
  public class Var<T> : IVar
    where T : struct
  {
    private object _rawValue;

    public VarKind Kind => GetKind(_rawValue) ?? VarKind.Null;

    public object RawValue
    {
      get => _rawValue;
      set
      {
        var kind = GetKind(_rawValue);
        if (kind != null)
          _rawValue = value;
        else
          throw new InvalidCastException($"O valor não é compatível com {this.GetType().Name}: {value.GetType().FullName}");
      }
    }

    private VarKind? GetKind(object value)
    {
      if (value == null)
        return VarKind.Null;

      if (value is T)
        return VarKind.Value;

      if (value is IList<T>)
        return VarKind.List;

      if (value is IDictionary<string, T>)
        return VarKind.Map;

      if (value is Range<T>)
        return VarKind.Range;

      return null;
    }

    public bool IsNull => _rawValue == null;

    public T Value
    {
      get => (_rawValue is T) ? (T)_rawValue : default(T);
      set => _rawValue = value;
    }

    public IList<T> List
    {
      get => _rawValue as IList<T>;
      set => _rawValue = value;
    }

    public IDictionary<string, T> Map
    {
      get => _rawValue as IDictionary<string, T>;
      set => _rawValue = value;
    }

    public Range<T> Range
    {
      get => (_rawValue is Range<T>) ? (Range<T>)_rawValue : default(Range<T>);
      set => _rawValue = value;
    }

    public static implicit operator Var<T>(T value)
    {
      return new Var<T> { Value = value };
    }

    public static implicit operator Var<T>(List<T> list)
    {
      return new Var<T> { List = list };
    }

    public static implicit operator Var<T>(T[] list)
    {
      return new Var<T> { List = list };
    }

    public static implicit operator Var<T>(Dictionary<string, T> map)
    {
      return new Var<T> { Map = map };
    }

    public static implicit operator Var<T>(Map<string, T> map)
    {
      return new Var<T> { Map = map };
    }

    public static implicit operator Var<T>(Range<T> range)
    {
      return new Var<T> { Range = range };
    }

    public override string ToString()
    {
      return _rawValue?.ToString() ?? "";
    }

    #region Implementação de IVar

    Type IVar.VarType => typeof(T);

    object IVar.Value
    {
      get => Value;
      set => Value = Change.To<T>(value);
    }

    bool IVar.HasWildcards => false;

    string IVar.TextPattern => null;

    IEnumerable IVar.List
    {
      get => List;
      set
      {
        if (value != null && !(value is IList<T>))
          throw new InvalidCastException($"O tipo não é compatível com {typeof(IList<T>).Name}: {value.GetType().FullName}");

        List = (IList<T>)value;
      }
    }

    IEnumerable IVar.Map
    {
      get => Map;
      set
      {
        if (value != null && !(value is IDictionary<string, T>))
          throw new InvalidCastException($"O tipo não é compatível com {typeof(IDictionary<string, T>).Name}: {value.GetType().FullName}");

        Map = (IDictionary<string, T>)value;
      }
    }

    Range IVar.Range
    {
      get => Range;
      set => Range = value;
    }

    #endregion
  }
}
