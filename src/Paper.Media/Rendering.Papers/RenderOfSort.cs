using System;
using System.Collections;
using System.Linq;
using Paper.Media.Design;
using Paper.Media.Design.Papers;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Papers
{
  static class RenderOfSort
  {
    public static void SetArgs(IPaper paper, PaperContext ctx)
    {
      var sort = paper._Get<Sort>("Sort");
      if (sort == null)
      {
        if (!paper._CanWrite("Sort"))
          return;

        sort = paper._SetNew<Sort>("Sort");
      }
      sort.CopyFromUri(ctx.RequestUri);
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      // nada a fazer por enquanto...
    }
  }
}