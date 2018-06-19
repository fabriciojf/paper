using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Métodos utilitários para textos.
  /// </summary>
  internal static class StringExtensions
  {
    private static readonly Regex numbers = new Regex("^[0-9]+$");

    /// <summary>
    /// Substitui um intervalo de texto.
    /// </summary>
    /// <param name="text">O texto a ser modificado.</param>
    /// <param name="start">A posição inicial para início da substituição.</param>
    /// <param name="length">A quantidade de caractres que serão substituídos.</param>
    /// <param name="replacement">O texto substituto.</param>
    /// <returns>O texto com a substituição aplicada.</returns>
    public static string Stuff(this string text, int start, int length, string replacement)
    {
      return
        text.Substring(0, start)
      + replacement
      + text.Substring(start + length);
    }

    public static bool IsNumber(this string text)
    {
      return numbers.IsMatch(text);
    }
  }
}
