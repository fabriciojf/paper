using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Interface da fábrica de objetos.
  /// 
  /// A fábrica deve ser capaz de detectar os parâmetros do objeto e
  /// encontra as instâncias registradas mais apropriadas para satisfazê-los.
  /// </summary>
  public interface IObjectFactory
  {
    /// <summary>
    /// Cria uma instância do objeto.
    /// A fábrica de objetos resolve os parâmetros do construtor.
    /// </summary>
    /// <param name="type">O tipo a ser instanciado.</param>
    /// <param name="args">
    /// Os argumentos providos.
    /// Os demais argumentos serão inferidos pela fábrica de objetos.
    /// </param>
    /// <returns>A instância criada.</returns>
    object CreateInstance(Type type, params object[] args);
  }
}