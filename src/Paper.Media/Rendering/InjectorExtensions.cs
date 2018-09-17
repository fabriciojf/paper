using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Rendering
{
  /// <summary>
  /// Extensões para o injetor de dependências.
  /// </summary>
  public static class InjectorExtensions
  {
    /// <summary>
    /// Instancia o tipo indicado resolvendo os parâmetros indicados no construtor.
    /// </summary>
    /// <typeparam name="T">O tipo a ser instanciado.</typeparam>
    /// <param name="args">Argumentos adicionais do construtor não providos pelo injetor.</param>
    /// <returns>O tipo instanciado.</returns>
    public static T CreateInstance<T>(this IInjector injector, params object[] args)
    {
      return (T)injector.CreateInstance(typeof(T), args);
    }
  }
}