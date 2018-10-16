using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Provedor de objetos do Paper.
  /// A instância do Provedor varia segundo o framework de implementação do HOST e
  /// provê as instâncias dos objetos fundamentais para renderização das rotas.
  /// </summary>
  public interface IProvider
  {
    /// <summary>
    /// Instância do utilitário de pesquisa de catálogos.
    /// O utilitário permite acesso ao catálogos dos sites conectados ao Paper.
    /// </summary>
    /// <returns>Instância do utilitário de pesquisa de catálogos.</returns>
    ICatalogSearcher GetCatalogCollection();

    /// <summary>
    /// Instância do rederizador de rotas do Paper.
    /// </summary>
    /// <param name="blueprint">Instância do Paper a ser renderizado.</param>
    /// <returns>Instância do renderizador do Paper.</returns>
    IRenderer GetRenderer(PaperBlueprint blueprint);
  }
}