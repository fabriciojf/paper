using System;

namespace Paper.Media.Rendering
{
  /// <summary>
  /// Interface para o injetor de dependências.
  /// O injetor tem a capacidade de instanciar classes resolvendo os 
  /// parâmetros de construtor.
  /// </summary>
  public interface IInjector
  {
    /// <summary>
    /// Instancia o tipo indicado resolvendo os parâmetros indicados no construtor.
    /// </summary>
    /// <param name="intanceType">Os argumentos repassados para o tipo.</param>
    /// <param name="args">Argumentos adicionais do construtor não providos pelo injetor.</param>
    /// <returns>O tipo instanciado.</returns>
    object CreateInstance(Type intanceType, params object[] args);
  }
}