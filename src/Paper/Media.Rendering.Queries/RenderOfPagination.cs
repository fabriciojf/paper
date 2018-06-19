using System.Linq;
using Paper.Media.Design;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Queries
{
  static class RenderOfPagination
  {
    public static void SetPagination(RenderContext ctx)
    {
      var pagination = ctx.Query.Get<Pagination>("Pagination");
      if (pagination == null)
      {
        if (!ctx.Query.IsWritable("Pagination"))
          return;

        pagination = ctx.Query.SetNew<Pagination>("Pagination");
      }

      var args = ctx.QueryArgs;
      int limit = args["limit"] is int ? (int)args["limit"] : 0;
      int offset = args["offset"] is int ? (int)args["offset"] : 0;
      int page = args["page"] is int ? (int)args["page"] : 0;

      if (limit > 0)
        pagination.Limit = limit;
      if (offset > 0)
        pagination.Offset = offset;
      if (page > 0) // o parametro "page" tem precedência sobre "offset"
        pagination.Offset = page * pagination.Limit;

      // forçando a consulta de um item a mais para detectar mais dados
      // disponiveis no lado do servidor
      pagination.Limit++;
    }

    public static void PosRenderPagination(RenderContext ctx)
    {
      var pagination = ctx.Query.Get<Pagination>("Pagination");
      if (pagination == null)
        return;

      var rowCount = ctx.Entity.Entities?.Count(e => e.Rel?.Contains("row") == true);
      var hasMoreRows = rowCount == pagination.Limit;
      pagination.Limit--;

      if (hasMoreRows)
      {
        // removendo a linha excedente
        var excess = ctx.Entity.Entities.Last(e => e.Rel.Contains("row"));
        ctx.Entity.Entities.Remove(excess);
      }

      var route = (Route)ctx.HttpContext.Request.GetRequestUri();

      var hasFirst = pagination.Offset >= (pagination.Limit << 1);
      var hasPrev = pagination.Offset > 0;
      var hasNext = hasMoreRows;

      if (ctx.Entity.Links == null)
      {
        ctx.Entity.Links = new LinkCollection();
      }

      if (hasFirst)
      {
        var limit = pagination.Limit;
        var offset = 0;
        var page = offset / limit;
        var href = route.SetArg("limit", limit).SetArg("offset", offset).SetArg("page", page);
        ctx.Entity.Links.Add(KnownRelations.First, "Início", href);
      }

      if (hasPrev)
      {
        var limit = pagination.Limit;
        var offset = pagination.Offset - pagination.Limit;
        if (offset < 0)
        {
          offset = 0;
        }
        var page = offset / limit;
        var href = route.SetArg("limit", limit).SetArg("offset", offset).SetArg("page", page);
        ctx.Entity.Links.Add(KnownRelations.Previous, "Anterior", href);
      }

      if (hasNext)
      {
        var limit = pagination.Limit;
        var offset = pagination.Offset + pagination.Limit;
        var page = offset / limit;
        var href = route.SetArg("limit", limit).SetArg("offset", offset).SetArg("page", page);
        ctx.Entity.Links.Add(KnownRelations.Next, "Próxima", href);
      }
    }
  }
}