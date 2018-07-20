using System.Data;
using Toolset;
using Toolset.Reflection;
using Paper.Media.Design;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Media.Design.Extensions.Papers.Rendering
{
  static class RenderOfInfo
  {
    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
      //
      // Valores padrão
      //
      entity.AddClass(paper.GetType());
      entity.AddLinkSelf(ctx.RequestUri);

      //
      // Valores personalizados
      //
      if (paper._Has<string>("GetTitle"))
      {
        var result = paper._Call<string>("GetTitle");
        if (result != null)
        {
          entity.AddTitle(result);
        }
      }

      if (paper._Has<NameCollection>("GetClass"))
      {
        var result = paper._Call<NameCollection>("GetClass");
        if (result != null)
        {
          entity.AddClass(result);
        }
      }

      if (paper._Has<NameCollection>("GetRel"))
      {
        var result = paper._Call<NameCollection>("GetRel");
        if (result != null)
        {
          entity.AddRel(result);
        }
      }

      RenderProperties(paper, entity, ctx);
      RenderLinks(paper, entity, ctx);

      //
      // Finalizações
      //
      if (entity.Title == null)
      {
        entity.Title = Conventions.MakeClassTitle(paper.GetType());
      }
    }

    private static void RenderProperties(IPaper paper, Entity entity, PaperContext ctx)
    {
      if (!paper._Has("GetProperties"))
        return;

      var properties = paper._Call("GetProperties");

      var dataTable = properties as DataTable;
      if (dataTable != null)
      {
        if (dataTable.Rows.Count > 0)
        {
          DataRow row = dataTable.Rows[0];
          foreach (DataColumn col in dataTable.Columns)
          {
            var key = Conventions.MakeFieldName(col);
            var value = row[col];
            entity.AddProperty(key, value);
          }
        }
        return;
      }

      var dictionary = properties as IDictionary;
      if (dictionary != null)
      {
        foreach (string key in dictionary.Keys)
        {
          var value = dictionary[key];
          entity.AddProperty(key, value);
        }
        return;
      }

      foreach (var key in properties._GetPropertyNames())
      {
        var value = properties._Get(key);
        entity.AddProperty(key, value);
      }
    }

    private static void RenderLinks(IPaper paper, Entity entity, PaperContext ctx)
    {
      if (!paper._Has<IEnumerable<ILink>>("GetLinks"))
        return;

      var linkRenderers = paper._Call<IEnumerable<ILink>>("GetLinks");
      foreach (var linkRenderer in linkRenderers)
      {
        var link = linkRenderer.RenderLink(ctx);
        if (link != null)
        {
          entity.AddLink(link);
        }
      }
    }
  }
}