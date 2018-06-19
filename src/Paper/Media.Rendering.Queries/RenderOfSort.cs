using System.Collections;
using System.Linq;
using Paper.Media.Design;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Queries
{
  static class RenderOfSort
  {
    public static void SetSort(RenderContext ctx)
    {
      var sort = ctx.Query.Get<Sort>("Sort");
      if (sort == null)
      {
        if (!ctx.Query.IsWritable("Sort"))
          return;

        sort = ctx.Query.SetNew<Sort>("Sort");
      }

      var value = ctx.QueryArgs["sort"];

      if (value == null)
        return;

      var sortings = (value as IEnumerable ?? new[] { value }).OfType<string>();

      foreach (var sorting in sortings)
      {
        var tokens = sorting.Split(':');
        var field = tokens.FirstOrDefault().Trim();
        var order = tokens.Skip(1).FirstOrDefault()?.Trim();
        var descending = (order == "desc") || (order == "descending");
        sort.AddSort(field, descending ? Sort.Order.Descending : Sort.Order.Ascending);
      }
    }
  }
}