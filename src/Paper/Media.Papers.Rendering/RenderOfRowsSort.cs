using System.Collections;
using System.Linq;
using Paper.Media.Design;
using Paper.Media.Design.Extensions;
using Media.Design.Extensions.Papers;
using Media.Design.Extensions.Papers.Rendering;
using Toolset.Collections;
using Toolset.Reflection;

namespace Media.Design.Extensions.Papers.Rendering
{
  static class RenderOfRowsSort
  {
    public static void SetArgs(IPaper paper, PaperContext ctx)
    {
      var sort = paper._Get<Sort>("RowsSort");
      if (sort == null)
      {
        if (!paper._CanWrite("RowsSort"))
          return;

        sort = paper._SetNew<Sort>("RowsSort");
      }
      sort.CopyFromUri(ctx.RequestUri, "sort");
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      var sort = paper._Get<Sort>("RowsSort");
      if (sort == null)
        return;

      var fieldNames = sort.Names.ChangeTo<CaseVariantString>();
      entity.AddProperty("__rowSort", fieldNames);
    }
  }
}