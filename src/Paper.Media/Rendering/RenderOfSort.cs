using System;
using System.Collections;
using System.Linq;
using Paper.Media.Design;
using Paper.Media.Design.Papers;
using Paper.Media.Routing;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Rendering
{
  static class RenderOfSort
  {
    /// <summary>
    /// Renderizador de propriedades de ordenação.
    /// </summary>
    /// <param name="paper">A instância do Paper com as definições de rederização da rota.</param>
    /// <param name="context">O contexto de rederização do Paper.</param>
    public static void SetArgs(IPaper paper, IContext context)
    {
      var sort = paper._Get<Sort>("Sort");
      if (sort == null)
      {
        if (!paper._CanWrite("Sort"))
          return;

        sort = paper._SetNew<Sort>("Sort");
      }
      sort.CopyFromUri(context.RequestUri);
    }

    public static void Render(IPaper paper, IContext context, Entity entity)
    {
      // nada a fazer por enquanto...
    }
  }
}