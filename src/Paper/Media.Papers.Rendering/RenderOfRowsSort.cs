using System.Collections;
using System.Linq;
using Paper.Media.Design;
using Paper.Media.Papers;
using Paper.Media.Papers.Rendering;
using Toolset.Reflection;

namespace Paper.Media.Papers.Rendering
{
  static class RenderOfRowsSort
  {
    public static void SetArgs(IPaper paper, PaperContext ctx)
    {
      var sort = paper._Get<Sort>("RowSort");
      if (sort == null)
      {
        if (!paper._CanWrite("RowSort"))
          return;

        sort = paper._SetNew<Sort>("RowSort");
      }
      sort.CopyFromUri(ctx.RequestUri, "sort");
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      var sort = paper._Get<Sort>("RowSort");
      if (sort == null)
        return;

      var fieldNames = sort.Select(x => (CaseVariantString)x.Name).ToArray();
      entity.AddProperty("__rowSort", fieldNames);
    }
  }
}