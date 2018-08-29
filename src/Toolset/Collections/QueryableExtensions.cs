using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Collections
{
  /// <summary>
  /// Extensão de funcionalidades para IQueryable.
  /// </summary>
  public static class QueryableExtensions
  {
    /// <summary>
    /// Tenta converter o item para o tipo indicado.
    /// Uma exceção é lançada se o tipo não for conversível destino.
    /// </summary>
    /// <typeparam name="T">O tipo destino da conversão.</typeparam>
    /// <param name="enumerable">Os itens a serem convertidos.</param>
    /// <returns>O enumerado dos itens convertidos.</returns>
    public static IQueryable<TType> ChangeTo<TType>(this IQueryable enumerable)
    {
      return enumerable.Cast<object>().Select(x => Change.To<TType>(x));
    }

    /// <summary>
    /// Exclui uma instância do enumrado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="enumerable">O enumerador de objetos.</param>
    /// <param name="single">O objeto a ser adicionado</param>
    /// <returns>O enumerado sem o objeto indicado.</returns>
    public static IQueryable<T> Except<T>(this IQueryable<T> enumerable, T single)
    {
      return enumerable.Except(new[] { single });
    }

    /// <summary>
    /// Adiciona um item ao início de um enumerado.
    /// </summary>
    /// <typeparam name="T">Tipo do enumerado.</typeparam>
    /// <param name="instance">Instância adicionado ao início do enumerado.</param>
    /// <returns>O enumerado contendo o item adcionado.</returns>
    public static IQueryable<T> Prepend<T>(this IQueryable<T> enumerable, T instance)
    {
      return new[] { instance }.AsQueryable().Concat(enumerable);
    }

#if !NETCOREAPP2_0
    /// <summary>
    /// Adiciona um item ao fim de um enumerado.
    /// </summary>
    /// <typeparam name="T">Tipo do enumerado.</typeparam>
    /// <param name="instance">Instância adicionado ao final do enumerado.</param>
    /// <returns>O enumerado contendo o item adcionado.</returns>
    public static IQueryable<T> Append<T>(this IQueryable<T> enumerable, T instance)
    {
      return enumerable.Concat(new[] { instance });
    }
#endif

    /// <summary>
    /// Emite apenas os itens não nulos.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao-nulos.</returns>
    public static IQueryable<T> NonNull<T>(this IQueryable<T> enumerable)
    {
      return (enumerable == null)
        ? Enumerable.Empty<T>().AsQueryable()
        : enumerable.Where(x => x != null && !(x is DBNull));
    }

    /// <summary>
    /// Emite apenas os itens não nulos e não vazios.
    /// Texto nulo ou vazio é considerado vazio.
    /// Vetores e coleções nulas ou vazias são consideradas vazias.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao-vazios.</returns>
    public static IQueryable<T> NonEmpty<T>(this IQueryable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>().AsQueryable();

      // As condicoes abaixo aceitam apenas itens considerados não nulos e não vazios
      return enumerable
        .Where(x => !(x == null))
        .Where(x => !(x is DBNull))
        .Where(x => !(x is string && string.IsNullOrEmpty(x as string)))
        // MoveNext é usado para testar se existem itens no enumerado
        .Where(x => !(x is IQueryable && !((IEnumerable)x).GetEnumerator().MoveNext()))
        .Where(x => !(x is IEnumerable && !((IEnumerable)x).GetEnumerator().MoveNext()));
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
    public static IQueryable<T> NonWhitespace<T>(this IQueryable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>().AsQueryable();

      // As condicoes abaixo aceitam apenas itens considerados não nulos, não vazios e não espaço em branco
      return enumerable
        .Where(x => !(x == null))
        .Where(x => !(x is DBNull))
        .Where(x => !(x is string && string.IsNullOrWhiteSpace(x as string)))
        .Where(x => !(x is IQueryable && !((IQueryable)x).Cast<object>().Any()))
        .Where(x => !(x is IEnumerable && !((IEnumerable)x).Cast<object>().Any()));
    }

    /// <summary>
    /// Emite os itens de uma coleção como um novo enumerado.
    /// Equivale à forma "enumerable.SelectMany(x => x)"
    /// </summary>
    /// <typeparam name="T">O tipo do enumerato.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <returns>Os itens da coleção enumerados.</returns>
    public static IQueryable<T> SelectMany<T>(this IQueryable<IQueryable<T>> enumerable)
    {
      return enumerable.SelectMany(x => x);
    }

    /// <summary>
    /// Varre o enumerado e executa a ação para cada item encontrado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">O enumerado dos itens.</param>
    /// <param name="action">A ação a ser executada.</param>
    public static void ForEach<T>(this IQueryable<T> enumerable, Action<T> action)
    {
      foreach (var item in enumerable)
      {
        action.Invoke(item);
      }
    }

    /// <summary>
    /// Varre o enumerado e executa a ação para cada item encontrado, repassando como parâmetro o
    /// item do enumerado mais o seu índice..
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerado dos itens.</param>
    /// <param name="action">A ação a ser executada.</param>
    public static void ForEach<T>(this IQueryable<T> enumerable, Action<T, int> action)
    {
      var items = enumerable.Select((element, index) => new { element, index });
      foreach (var item in items)
      {
        action.Invoke(item.element, item.index);
      }
    }
  }
}
