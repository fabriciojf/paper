using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Extensões para automação de código.
  /// </summary>
  internal static class Extensions
  {
    /// <summary>
    /// Cria um enumerado contendo um único objeto.
    /// Útil para incorporar objetos simples em expressões LINQ.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="target">O objeto a ser convertido.</param>
    /// <returns>O enumerado contendo o objeto.</returns>
    public static IEnumerable<T> AsSingle<T>(this T target)
    {
      yield return target;
    }
    /// <summary>
    /// Aplica um método para cada item encontrado em um enumerado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="action">A ação a ser aplicada.</param>
    /// <returns>
    /// O próprio enumerado recebido.
    /// Útil para o encadeamento de expressões.
    /// </returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var item in enumerable.ToArray())
      {
        action.Invoke(item);
      }
      return enumerable;
    }
    /// <summary>
    /// Converte um texto para o formato 'hifenizado':
    /// -   Texto em minúsculo.
    /// -   Palavras separadas por hífen.
    /// -   Como em: texto-hifenizado
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Hyphenate(this string text)
    {
      return Regex.Replace(text, "([A-Z])", "-$1").ToLower().Substring(1);
    }
    /// <summary>
    /// Une as linhas em um texto separando linhas por quebras de linha.
    /// </summary>
    public static string JoinLines(this IEnumerable<string> lines)
    {
      return string.Join(Environment.NewLine, lines);
    }
  }
}
