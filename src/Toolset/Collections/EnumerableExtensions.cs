using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Collections
{
  /// <summary>
  /// Extensão de funcionalidades para IEnumerable.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Exclui uma instância do enumrado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="enumerable">O enumerador de objetos.</param>
    /// <param name="single">O objeto a ser adicionado</param>
    /// <returns>O enumerado sem o objeto indicado.</returns>
    public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T single)
    {
      return enumerable.Except(new[] { single });
    }

    /// <summary>
    /// Emite um enumerado contendo apenas o objeto referenciado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="single">A instância do objeto.</param>
    /// <returns>Um enumerado contendo o objeto.</returns>
    public static IEnumerable<T> AsSingle<T>(this T single)
    {
      yield return single;
    }

    /// <summary>
    /// Emite apenas os itens não nulos.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao-nulos.</returns>
    public static IEnumerable<T> NonNull<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>();
      return enumerable.Where(x => x != null && !(x is DBNull));
    }

    /// <summary>
    /// Emite apenas os itens não nulos e não vazios.
    /// Texto nulo ou vazio é considerado vazio.
    /// Vetores e coleções nulas ou vazias são consideradas vazias.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao-vazios.</returns>
    public static IEnumerable<T> NonEmpty<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>();
      return enumerable.Where(
        x =>
          (x == null || x is DBNull)
            ? false
            : (x is string)
              ? !string.IsNullOrEmpty(x as string)
              : (x is IEnumerable)
                ? ((IEnumerable)x).GetEnumerator().MoveNext()
                : true
      );
    }

    /// <summary>
    /// Emite apenas os itens não nulos, não vazios e não em branco.
    /// Texto nulo, vazio ou contendo apenas espaços, tabulações e quebras de linha
    /// são considerados em branco.
    /// Vetores e coleções nulas ou vazias são consideradas em branco.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao em branco.</returns>
    public static IEnumerable<T> NonWhitespace<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>();
      return enumerable.Where(
        x =>
          (x == null || x is DBNull)
            ? false
            : (x is string)
              ? !string.IsNullOrWhiteSpace(x as string)
              : (x is IEnumerable)
                ? ((IEnumerable)x).GetEnumerator().MoveNext()
                : true
      );
    }

    /// <summary>
    /// Emite os itens de uma coleção como um novo enumerado.
    /// Equivale à forma "enumerable.SelectMany(x => x)"
    /// </summary>
    /// <typeparam name="T">O tipo do enumerato.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <returns>Os itens da coleção enumerados.</returns>
    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
      return enumerable.SelectMany(x => x);
    }

    /// <summary>
    /// Varre o enumerado e executa a ação para cada item encontrado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">O enumerado dos itens.</param>
    /// <param name="action">A ação a ser executada.</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var item in enumerable)
      {
        action.Invoke(item);
      }
    }
  }
}
