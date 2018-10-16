using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Utilitário de pesquisa de catálogos do Paper.
  /// 
  /// Cada site conectado ao Paper possui seu próprio catálogo.
  /// Este utilitário pode ser usado para resolver o catálogo mais apropriado
  /// para cada rota.
  /// </summary>
  public interface ICatalogSearcher
  {
    /// <summary>
    /// Pesquisa o catálogo do site registrado no caminho indicado.
    /// O caminho indica a raiz de um site registrado no Paper.
    /// Caso o caminho não aponte para a raiz de um site nenhum catálogo é retornado.
    /// </summary>
    /// <param name="path">
    /// O caminho de registro de um site conectado ao Paper, na forma "/Caminho/Site".
    /// </param>
    /// <returns>O catálogo mais apropriado para renderizar Papers da rota.</returns>
    ICatalog SearchCatalog(string path);
  }
}