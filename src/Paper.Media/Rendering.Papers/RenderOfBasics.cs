using System.Data;
using Toolset;
using Toolset.Reflection;
using Paper.Media.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using Paper.Media.Design.Papers;
using Media.Utilities.Types;

namespace Paper.Media.Rendering.Papers
{
  static class RenderOfBasics
  {
    /// <summary>
    /// Repassa os argumentos indicados para as propriedades na instância de IPaper.
    /// </summary>
    /// <param name="paper">A instância de IPaper que será modificada.</param>
    /// <param name="args">A coleção dos argumentos atribuídos.</param>
    public static void SetArgs(IPaper paper, ArgMap args)
    {
      foreach (var arg in args)
      {
        if (arg.Value is ArgMap)
          continue;

        paper._TrySet(arg.Key, arg.Value);
      }
    }

    public static void Render(IPaper paper, Entity entity, PaperContext ctx)
    {
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
        entity.Title = Conventions.MakeTitle(paper.GetType());
      }
    }

    private static void RenderProperties(IPaper paper, Entity entity, PaperContext ctx)
    {
      if (!paper._Has("GetProperties"))
        return;

      var properties = paper._Call("GetProperties");
      if (properties == null)
        return;

      var dataTable = properties as DataTable;
      if (dataTable != null)
      {
        if (dataTable.Rows.Count > 0)
        {
          DataRow row = dataTable.Rows[0];
          foreach (DataColumn col in dataTable.Columns)
          {
            var key = Conventions.MakeName(col);
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
      if (linkRenderers == null)
        return;

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