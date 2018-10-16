using System;
using System.Linq;
using Media.Utilities.Types;
using Paper.Media.Design;
using Paper.Media.Routing;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering
{
  static class RenderOfPage
  {
    /// <summary>
    /// Renderizador de propriedades de paginação.
    /// </summary>
    /// <param name="paper">A instância do Paper com as definições de rederização da rota.</param>
    /// <param name="context">O contexto de rederização do Paper.</param>
    public static void SetArgs(IPaper paper, IContext context)
    {
      if (!paper._Has("Page"))
        return;

      var page = paper._Get<Page>("Page");
      if (page == null)
      {
        if (!paper._CanWrite("Page"))
          return;

        page = page._SetNew<Page>("Page");
      }

      page.CopyFromUri(context.RequestUri);

      context.Cache.Set(CacheKeys.Page, page);
    }

    /// <summary>
    /// Forçando a consulta de um item a mais para detectar mais dados
    /// disponiveis no lado do servidor
    /// </summary>
    public static void PreCache(IPaper paper, IContext context)
    {
      var page = context.Cache.Get<Page>(CacheKeys.Page);
      if (page == null)
        return;

      page.SetLimitOrSize(page.Limit + 1);
    }

    /// <summary>
    /// Detectando o registro excedente.
    /// Se existir, então, temos mais registros no lado do servidor.
    /// </summary>
    public static void PostCache(IPaper paper, IContext context)
    {
      var hasMoreData = false;

      var page = context.Cache.Get<Page>(CacheKeys.Page);
      if (page == null)
        return;

      var overSize = page.Limit;
      // Voltando o tamanho da página para o original.
      page.SetLimitOrSize(page.Limit - 1);

      var rows = context.Cache.Get<DataWrapperEnumerable>(CacheKeys.Rows);
      if (rows != null)
      {
        hasMoreData = (rows.Count == overSize);
        if (hasMoreData)
        {
          // Reduzindo a quantidade de registros que deve ser lidos
          // para provocar o descarte do excedente.
          rows.Count--;
        }
      }

      context.Cache.Set(CacheKeys.HasMoreData, hasMoreData);
    }

    public static void Render(IPaper paper, IContext context, Entity entity)
    {
      var pagination = context.Cache.Get<Page>(CacheKeys.Page);
      var hasMoreRows = context.Cache.Get<bool>(CacheKeys.HasMoreData);

      if (pagination == null)
        return;

      var hasFirst = pagination.Number > 1;
      var hasPrev = pagination.Number > 1;
      var hasNext = hasMoreRows;

      var route = new Route(context.RequestUri);

      if (hasFirst)
      {
        var page = pagination.FirstPage();
        var href = page.CopyToUri(context.RequestUri);
        entity.AddLink(href, "Início", Rel.First);
      }

      if (hasPrev)
      {
        var page = pagination.PreviousPage();
        var href = page.CopyToUri(context.RequestUri);
        entity.AddLink(href, "Anterior", Rel.Previous);
      }

      if (hasNext)
      {
        var page = pagination.NextPage();
        var href = page.CopyToUri(context.RequestUri);
        entity.AddLink(href, "Próxima", Rel.Next);
      }
    }
  }
}