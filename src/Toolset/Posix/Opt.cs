using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Tipo para opção booliana.
  /// Quando presente na linha de comando:
  /// - On é marcado como verdadeiro.
  /// </summary>
  public class Opt
  {
    internal Opt()
    {
    }

    internal Opt(bool on)
    {
      this.On = true;
    }

    public bool On { get; set; }

    public override string ToString() => On.ToString();
    public static implicit operator bool(Opt arg) => arg.On;
    public static implicit operator Opt(bool value) => new Opt(value);
  }
}
