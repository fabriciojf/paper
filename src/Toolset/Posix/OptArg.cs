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
  public class OptArg : Opt
  {
    internal OptArg()
    {
    }

    internal OptArg(string text)
    {
      this.On = true;
      this.Text = text;
    }

    public virtual string Text { get; set; }

    public override string ToString() => Text;
    public static implicit operator string(OptArg arg) => arg.Text;
    public static implicit operator OptArg(string value) => new OptArg(value);
    public static implicit operator bool(OptArg arg) => arg.On;
  }
}
