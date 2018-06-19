using System.Data;
using Toolset;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Queries
{
  static class RenderOfCommons
  {
    public static void SetArgs(RenderContext ctx)
    {
      foreach (var arg in ctx.PathArgs)
      {
        if (ctx.Query.IsWritable(arg.Key))
        {
          ctx.Query.Set(arg.Key, arg.Value);
        }
      }
    }

    public static void RenderInfo(RenderContext ctx)
    {
      ctx.Entity.Class = new NameCollection();

      var entityClass = Conventions.MakeClassName(ctx.Query);
      if (entityClass != null)
        ctx.Entity.Class.Add(entityClass);

      ctx.Entity.Links = new LinkCollection();
      ctx.Entity.Links.AddSelf(ctx.HttpContext.Request.GetRequestUri());
    }

    public static void RenderClass(RenderContext ctx)
    {
      var classes = ctx.Query.Call("GetClass");
      if (classes == null)
        return;

      if (ctx.Entity.Class == null)
        ctx.Entity.Class = new NameCollection();

      var names = (classes as NameCollection) ?? new NameCollection(classes.ToString());
      ctx.Entity.Class.AddMany(names);
    }

    public static void RenderRels(RenderContext ctx)
    {
      var rels = ctx.Query.Call("GetRels");
      if (rels == null)
        return;

      if (ctx.Entity.Rel == null)
        ctx.Entity.Rel = new NameCollection();

      var names = (rels as NameCollection) ?? new NameCollection(rels.ToString());
      ctx.Entity.Rel.AddMany(names);
    }

    public static void RenderTitle(RenderContext ctx)
    {
      var title = ctx.Query.Call("GetTitle")?.ToString();
      if (title == null)
      {
        title = ctx.Query.GetType().Name
          .Replace("Page", "")
          .Replace("Query", "")
          .ChangeCase(TextCase.ProperCase);
      }
      ctx.Entity.Title = title;
    }

    public static void RenderProperties(RenderContext ctx)
    {
      var properties = ctx.Query.Call("GetProperties");
      if (properties == null)
        return;

      if (ctx.Entity.Properties == null)
      {
        ctx.Entity.Properties = new Media.PropertyCollection();
      }

      if (properties is DataTable)
      {
        var data = (DataTable)properties;
        if (data.Rows.Count > 0)
        {
          DataRow row = data.Rows[0];
          foreach (DataColumn col in data.Columns)
          {
            var value = row[col];
            if (value != null)
            {
              var name = Conventions.MakeFieldName(col);
              ctx.Entity.Properties.Add(name, value);
            }
          }
        }
      }
      else
      {
        var props = properties.GetType().GetProperties();
        foreach (var prop in props)
        {
          var value = prop.GetValue(properties);
          if (value != null)
          {
            var name = Conventions.MakeFieldName(prop.Name);
            ctx.Entity.Properties.Add(name, value);
          }
        }
      }
    }

    public static void RenderLinks(RenderContext ctx)
    {
      LinkRenderer.RenderLinks(ctx);
    }

  }
}