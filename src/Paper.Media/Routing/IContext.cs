using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Contexto de rederização de rota do Paper.
  /// </summary>
  public interface IContext
  {
    /// <summary>
    /// Argumentos extraídos da URI de requisição pela aplicação do template de URI.
    /// </summary>
    IArgs RequestArgs { get; }

    /// <summary>
    /// URI de requisição.
    /// </summary>
    RequestUri RequestUri { get; }

    /// <summary>
    /// Provedor de objetos do Paper.
    /// </summary>
    IProvider Provider { get; }

    /// <summary>
    /// Catálogo dos Papers conhecidos.
    /// Cada Paper contém instruções de uma rota específica.
    /// </summary>
    ICatalog Catalog { get; }

    /// <summary>
    /// Instância da fábrica de objetos pela injeção de dependência.
    /// </summary>
    IObjectFactory Factory { get; }

    /// <summary>
    /// Cache de objetos durante uma requisição.
    /// </summary>
    ICache Cache { get; }
  }
}