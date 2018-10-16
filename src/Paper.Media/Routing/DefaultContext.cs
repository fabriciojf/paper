using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Contexto de rederização de rota do Paper.
  /// </summary>
  public class DefaultContext : IContext
  {
    /// <summary>
    /// Argumentos extraídos da URI de requisição pela aplicação do template de URI.
    /// </summary>
    public IArgs RequestArgs { get; set; }

    /// <summary>
    /// Uri de requisição.
    /// </summary>
    public RequestUri RequestUri { get; set; }

    /// <summary>
    /// Provedor de objetos do Paper.
    /// </summary>
    public IProvider Provider { get; set; }

    /// <summary>
    /// Catálogo dos Papers conhecidos.
    /// Cada Paper contém instruções de uma rota específica.
    /// </summary>
    public ICatalog Catalog { get; set; }

    /// <summary>
    /// Instância da fábrica de objetos pela injeção de dependência.
    /// </summary>
    public IObjectFactory Factory { get; set; }

    /// <summary>
    /// Cache de objetos durante uma requisição.
    /// </summary>
    public ICache Cache { get; set; }
  }
}