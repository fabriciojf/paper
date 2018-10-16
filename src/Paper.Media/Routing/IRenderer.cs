using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;
using Toolset;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Utilitário de rederização de rotas do Paper.
  /// O utilitário deve interpretar os parâmetros da rota e produzir uma instância 
  /// de Entity como resposta.
  /// </summary>
  public interface IRenderer
  {
    /// <summary>
    /// Renderiza a rota produzindo a instância de Entity com as propriedades de hipermídia.
    /// </summary>
    /// <param name="paper">A instância do Paper com as definições de rederização da rota.</param>
    /// <param name="context">O contexto de rederização do Paper.</param>
    /// <returns>A instância de Entity com as propriedades de hipermídia.</returns>
    Ret<Entity> RenderEntity(IPaper paper, IContext context);
  }
}