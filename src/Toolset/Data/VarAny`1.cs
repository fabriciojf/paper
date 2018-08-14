using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;

namespace Toolset.Data
{
  public class VarAny<T> : IVar
    where T : class
  {
    private object _rawValue;

    public VarKinds Kind => GetKind(_rawValue) ?? VarKinds.Null;

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

    private VarKinds? GetKind(object value)
    {
      if (value == null)
        return VarKinds.Null;

      if (value is IList<T>)
        return VarKinds.List;

      if (value is IDictionary<string, T>)
        return VarKinds.Map;

      if (value is RangeEx<T>)
        return VarKinds.Range;

      if (value is T)
      {
        if (value.GetType().IsValueType)
          return VarKinds.Primitive;

        if (value is string)
          return VarKinds.Text;

        else
          return VarKinds.Graph;
      }

      return null;
    }

    public bool IsNull => _rawValue == null;

    public T Value
    {
      get => Kind.HasFlag(VarKinds.Value) ? _rawValue as T : null;
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

    public RangeEx<T> Range
    {
      get => (_rawValue is RangeEx<T>) ? (RangeEx<T>)_rawValue : default(RangeEx<T>);
      set => _rawValue = value;
    }

    public static implicit operator VarAny<T>(T text)
    {
      return new VarAny<T> { Value = text };
    }

    public static implicit operator VarAny<T>(List<T> list)
    {
      return new VarAny<T> { List = list };
    }

    public static implicit operator VarAny<T>(T[] list)
    {
      return new VarAny<T> { List = list };
    }

    public static implicit operator VarAny<T>(Dictionary<string, T> map)
    {
      return new VarAny<T> { Map = map };
    }

    public static implicit operator VarAny<T>(Map<string, T> map)
    {
      return new VarAny<T> { Map = map };
    }

    public static implicit operator VarAny<T>(RangeEx<T> range)
    {
      return new VarAny<T> { Range = range };
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
      set => Value = (T)value;
    }

    bool IVar.HasWildcards
    {
      get => Var.HasWildcards(Value as string);
    }

    string IVar.TextPattern
    {
      get => (Value is string) ? Var.CreateTextPattern(Value as string) : null;
    }

    IList IVar.List
    {
      get => (IList)List;
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
