using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;
using Toolset;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Catalogo de Papers.
  /// Um Paper carrega as instruções de renderização das páginas web.
  /// O catálogo contém todos os Papers conhecidos.
  /// </summary>
  public interface ICatalog
  {
    /// <summary>
    /// Obtém o Paper mapeado para a rota.
    /// </summary>
    /// <param name="route">
    /// A rota acessada.
    /// A rota pode ser um caminho de URI, como: /My/Page
    /// Ou uma URI completa, como: http://host.com/My/Page?q=x
    /// </param>
    /// <returns>O Paper para renderização da rota ou nulo.</returns>
    PaperBlueprint GetPaperBlueprint(string route);
  }
}