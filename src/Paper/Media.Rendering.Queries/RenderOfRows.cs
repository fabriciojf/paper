using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Queries
{
  static class RenderOfRows
  {
    #region RenderRows

    public static void RenderRows(RenderContext ctx)
    {
      if (!ctx.Query.HasMethod("GetRows"))
        return;

      if (ctx.Entity.Properties == null)
      {
        ctx.Entity.Properties = new Media.PropertyCollection();
      }

      if (ctx.Entity.Entities == null)
      {
        ctx.Entity.Entities = new EntityCollection();
      }

      ctx.Entity.Class.AddAt(0, KnownClasses.Rows);

      var data = ctx.Query.Call("GetRows");
      if (data == null)
        return;

      if (data is DataTable)
      {
        RenderRows_FromDataTable(ctx);
        InferDataHeaders_FromDataTable(ctx);
      }
      else
      {
        PropertyInfo[] seenProperties = RenderRows_FromEnumerable(ctx);
        InferDataHeaders_FromEnumerable(ctx, seenProperties);
      }

      UpdateRowsHeaders(ctx);
    }

    private static void RenderRows_FromDataTable(RenderContext ctx)
    {
      var data = (DataTable)ctx.Rows;
      foreach (DataRow row in data.Rows)
      {
        var rowEntity = new Entity();
        rowEntity.Class = KnownClasses.Row;
        rowEntity.Rel = KnownRelations.Row;
        rowEntity.Properties = new Media.PropertyCollection();
        foreach (DataColumn col in data.Columns)
        {
          var value = row[col];
          if (value != null)
          {
            var name = Conventions.MakeFieldName(col);
            rowEntity.Properties.Add(name, value);
          }
        }
        ctx.Entity.Entities.Add(rowEntity);

        LinkRenderer.RenderRowLinks(ctx, rowEntity, row);
      }
      ctx.Entity.Properties.Add("_rowCount", data.Rows.Count);
    }

    static PropertyInfo[] RenderRows_FromEnumerable(RenderContext ctx)
    {
      var data = ((IEnumerable)ctx.Rows).Cast<object>();
      var allProperties = new List<PropertyInfo>();

      var count = 0;
      foreach (var row in data)
      {
        var rowEntity = new Entity();
        rowEntity.Class = new NameCollection();
        rowEntity.Class.Add(KnownClasses.Row);

        var entityClass = Conventions.MakeClassName(row);
        if (entityClass != null)
          rowEntity.Class.Add(entityClass);

        rowEntity.Rel = KnownRelations.Row;
        rowEntity.Properties = new Media.PropertyCollection();

        var properties = row.GetType().GetProperties();
        foreach (var property in properties)
        {
          if (!allProperties.Contains(property))
          {
            allProperties.Add(property);
          }

          var value = property.GetValue(row);
          if (value != null)
          {
            var name = Conventions.MakeFieldName(property.Name);
            rowEntity.Properties.Add(name, value);
          }
        }
        
        LinkRenderer.RenderRowLinks(ctx, rowEntity, row);

        ctx.Entity.Entities.Add(rowEntity);
        count++;
      }
      ctx.Entity.Properties.Add("_rowCount", count);

      var knownProperties = (
        from property in allProperties
        let name = Conventions.MakeFieldName(property.Name)
        group property by name into g
        select g.First()
      ).ToArray();

      return knownProperties;
    }

    private static void InferDataHeaders_FromDataTable(RenderContext ctx)
    {
      var data = (DataTable)ctx.Rows;
      foreach (DataColumn col in data.Columns)
      {
        var name = Conventions.MakeFieldName(col);
        var title = Conventions.MakeFieldTitle(col);
        var type = Conventions.MakeFieldType(col);
        ctx.Entity.Properties.AddRowsHeader(name, title, type);
      }
    }

    private static void InferDataHeaders_FromEnumerable(RenderContext ctx, PropertyInfo[] properties)
    {
      foreach (var property in properties)
      {
        var name = Conventions.MakeFieldName(property.Name);
        var title = Conventions.MakeFieldTitle(property.Name);
        var type = Conventions.MakeFieldType(property.PropertyType);
        ctx.Entity.Properties.AddRowsHeader(name, title, type);
      }
    }

    #endregion

    #region UpdateRowsHeaders

    private static void UpdateRowsHeaders(RenderContext ctx)
    {
      var rowsHeaders = ctx.Query.Call("GetRowsHeaders");
      if (rowsHeaders == null)
        return;

      var isStrings = typeof(IEnumerable<string>).IsAssignableFrom(rowsHeaders.GetType());
      var isEnumerable = typeof(IEnumerable).IsAssignableFrom(rowsHeaders.GetType());

      if (isStrings)
      {
        UpdateRowsHeaders_FromStrings(ctx);
      }
      else if (isEnumerable)
      {
        UpdateRowsHeaders_FromEnumerable(ctx);
      }
      else
      {
        UpdateRowsHeaders_FromObject(ctx);
      }
    }

    private static void UpdateRowsHeaders_FromStrings(RenderContext ctx)
    {
      var rowsHeaders = ctx.Query.Call<IEnumerable<string>>("GetRowsHeaders");
      var entityHeaders = ctx.Entity.Properties.GetRowsHeaders();

      var rowsHeadersEnumerator = rowsHeaders.GetEnumerator();
      var entityHeadersEnumerator = entityHeaders.GetEnumerator();

      while (rowsHeadersEnumerator.MoveNext() && entityHeadersEnumerator.MoveNext())
      {
        var title = Conventions.MakeFieldTitle(rowsHeadersEnumerator.Current);
        entityHeadersEnumerator.Current.Title = title;
      }
    }

    private static void UpdateRowsHeaders_FromEnumerable(RenderContext ctx)
    {
      var rowsHeaders = ctx.Query.Call<IEnumerable>("GetRowsHeaders");
      var entityHeaders = ctx.Entity.Properties.GetRowsHeaders();

      var rowsHeadersEnumerator = rowsHeaders.GetEnumerator();
      var entityHeadersEnumerator = entityHeaders.GetEnumerator();

      while (rowsHeadersEnumerator.MoveNext() && entityHeadersEnumerator.MoveNext())
      {
        object item = rowsHeadersEnumerator.Current;
        Header header = entityHeadersEnumerator.Current;

        var name = item.Get<string>("Name");
        if (name != null)
        {
          name = Conventions.MakeFieldName(name);
          header = entityHeaders[name];
          if (header == null)
            continue; // A coluna referencia um campo nao encontrado nos registros
        }

        var oldHeaderName = header.Name;

        header.CopyFrom(item, CopyOptions.IgnoreNull);

        // corrigindo o nome da propriedade, caso tenha sido substituido
        header.Name = oldHeaderName;
      }
    }

    private static void UpdateRowsHeaders_FromObject(RenderContext ctx)
    {
      var rowsHeaders = ctx.Query.Call("GetRowsHeaders");
      var entityHeaders = ctx.Entity.Properties.GetRowsHeaders();

      var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

      var rowsHeadersEnumerator = rowsHeaders.GetType().GetProperties(flags).Cast<PropertyInfo>().GetEnumerator();
      var entityHeadersEnumerator = ctx.Entity.Properties.GetRowsHeaders().GetEnumerator();

      while (rowsHeadersEnumerator.MoveNext() && entityHeadersEnumerator.MoveNext())
      {
        Header header = entityHeadersEnumerator.Current;

        var property = rowsHeadersEnumerator.Current;
        var item = property.GetValue(rowsHeaders);

        var name = Conventions.MakeFieldName(property.Name);
        header = entityHeaders[name];
        if (header == null)
          continue; // A coluna referencia um campo nao encontrado nos registros

        if (item is string)
        {
          header.Title = (string)item;
        }
        else
        {
          var oldHeaderName = header.Name;

          header.CopyFrom(item, CopyOptions.IgnoreNull);

          // corrigindo o nome da propriedade, caso tenha sido substituido
          header.Name = oldHeaderName;
        }
      }
    }

    #endregion
  }
}