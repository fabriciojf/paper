using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Tipo para opção com argumento.
  /// Quando presente na linha de comando:
  /// - On é marcado como verdadeiro.
  /// - O próximo argumento na linha de comando é salvo em Value.
  /// </summary>
  public class OptArg<T> : OptArg
  {
    private object _value;

    internal OptArg()
    {
    }

    internal OptArg(T value)
    {
      this.On = true;
      this.Value = value;
    }

    public T Value
    {
      get { return (_value != null) ? (T)_value : default(T); }
      set { _value = value; }
    }

    public override string Text
    {
      get => _value?.ToString();
      set => _value = (value == null) ? null : Change.To(value, typeof(T));
    }

    public override string ToString() => Text;
    public static implicit operator T(OptArg<T> arg) => arg.Value;
    public static implicit operator OptArg<T>(T value) => new OptArg<T>(value);
    public static implicit operator bool(OptArg<T> arg) => arg.On;
  }
}
