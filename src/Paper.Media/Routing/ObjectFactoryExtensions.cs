using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Extensões para a fábrica de objetos.
  /// </summary>
  public static class ObjectFactoryExtensions
  {
    /// <summary>
    /// Cria um objeto resolvendo dependências.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto a ser instanciado.</typeparam>
    /// <param name="factory">A fábrica de objetos.</param>
    /// <param name="args">Os argumentos extras não resolvidos pela fábrica.</param>
    /// <returns>A instância do objeto criado.</returns>
    public static T CreateInstance<T>(this IObjectFactory factory, params object[] args)
    {
      return (T)factory.CreateInstance(typeof(T), args);
    }
  }
}