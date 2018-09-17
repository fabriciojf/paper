using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Media.Utilities.Types;
using Paper.Media.Design;
using Paper.Media.Design.Papers;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Papers
{
  static class RenderOfData
  {
    /// <summary>
    /// Realiza a consulta de dados specificados no Paper e estoca os dados
    /// obtidos no cache indicado.
    /// </summary>
    /// <param name="paper">A instância de IPaper que contém as consultas a dados.</param>
    /// <param name="cache">O cache para estocagem dos dados consultados.</param>
    public static void CacheData(IPaper paper, EntryCollection cache)
    {
      if (paper._Has("GetData"))
      {
        var data = paper._Call("GetData");
        if (data != null)
        {
          var dataWrapper = DataWrapper.Create(data);
          cache.Set(CacheKeys.Data, dataWrapper);
        }
      }
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      // Os dados foram renderizados anteriormente e estocados no cache
      var data = ctx.Cache.Get<DataWrapper>(CacheKeys.Data);
      if (data == null)
        return;

      entity.AddClass(Class.Data);

      AddData(paper, entity, ctx, data);
      AddDataHeaders(paper, entity, ctx, data);
      AddDataLinks(paper, entity, ctx, data);
    }

    /// <summary>
    /// Renderizando dados e cabeçalhos básicos
    /// </summary>
    private static void AddData(IPaper paper, Entity entity, PaperContext ctx, DataWrapper data)
    {
      foreach (var key in data.EnumerateKeys())
      {
        var value = data.GetValue(key);
        entity.AddProperty(key, value);

        var header = data.GetHeader(key);
        entity.AddDataHeader(header);
      }
    }

    /// <summary>
    /// Renderizando personalizações nos cabeçalhos
    /// </summary>
    private static void AddDataHeaders(IPaper paper, Entity entity, PaperContext ctx, DataWrapper data)
    {
      // Adicionando os campos personalizados
      //
      var headers = paper._Call<IEnumerable<HeaderInfo>>("GetDataHeaders", data.DataSource);
      if (headers != null)
      {
        if (!(headers is IList))
        {
          headers = headers.ToArray();
        }

        entity.AddDataHeaders(headers);

        // Ocultando as colunas não personalizadas
        //
        entity.ForEachDataHeader((e, h) =>
        {
          h.Hidden = !headers.Any(x => x.Name.EqualsIgnoreCase(h.Name));
        });
      }
    }

    /// <summary>
    /// Renderizando links
    /// </summary>
    private static void AddDataLinks(IPaper paper, Entity entity, PaperContext ctx, DataWrapper data)
    {
      var linkRenderers = paper._Call<IEnumerable<ILink>>("GetDataLinks", data.DataSource);
      if (linkRenderers != null)
      {
        foreach (var linkRenderer in linkRenderers)
        {
          var link = linkRenderer.RenderLink(ctx);
          if (link != null)
          {
            link.AddRel(RelNames.DataLink);
            link.Rel.Remove(RelNames.Link);
            entity.AddLink(link);
          }
        }
      }
    }
  }
}