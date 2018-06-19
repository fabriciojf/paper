using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Queries
{
  static class RenderOfData
  {
    #region RenderData

    public static void RenderData(RenderContext ctx)
    {
      if (!ctx.Query.HasMethod("GetData"))
        return;

      if (ctx.Entity.Class == null)
      {
        ctx.Entity.Class = new NameCollection();
      }
      if (ctx.Entity.Properties == null)
      {
        ctx.Entity.Properties = new Media.PropertyCollection();
      }

      ctx.Entity.Class.AddAt(0, KnownClasses.Data);

      var type = GetDataType(ctx);
      if (type == typeof(DataTable))
      {
        RenderData_FromDataTable(ctx);
        InferDataHeaders_FromDataTable(ctx);

        var data = (DataTable)ctx.Data;
        var row = data?.Rows.Cast<DataRow>().FirstOrDefault();
        if (row != null)
        {
          LinkRenderer.RenderDataLinks(ctx, row);
        }
      }
      else
      {
        RenderData_FromObject(ctx);
        InferDataHeaders_FromObject(ctx);

        if (ctx.Data != null)
        {
          LinkRenderer.RenderDataLinks(ctx, ctx.Data);
        }
      }

      UpdateDataHeaders(ctx);
    }

    private static void RenderData_FromDataTable(RenderContext ctx)
    {
      var data = (DataTable)ctx.Data;
      if ((data?.Rows.Count ?? 0) > 0)
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
      ctx.Entity.Properties.Add("_dataCount", data?.Columns.Count ?? 0);
    }

    private static void RenderData_FromObject(RenderContext ctx)
    {
      var data = ctx.Data;
      if (data != null)
      {
        var properties = data.GetType().GetProperties();
        foreach (var property in properties)
        {
          var value = property.GetValue(data);
          if (value != null)
          {
            var name = Conventions.MakeFieldName(property.Name);
            ctx.Entity.Properties.Add(name, value);
          }
        }
      }
    }

    private static void InferDataHeaders_FromDataTable(RenderContext ctx)
    {
      var data = (DataTable)ctx.Data;
      foreach (DataColumn col in data.Columns)
      {
        var name = Conventions.MakeFieldName(col);
        var title = Conventions.MakeFieldTitle(col);
        var type = Conventions.MakeFieldType(col);
        ctx.Entity.Properties.AddDataHeader(name, title, type);
      }
    }

    private static void InferDataHeaders_FromObject(RenderContext ctx)
    {
      var dataType = GetDataType(ctx);
      if (dataType == typeof(object))
        return;

      var properties = dataType.GetProperties();
      foreach (var property in properties)
      {
        var name = Conventions.MakeFieldName(property.Name);
        var title = Conventions.MakeFieldTitle(property.Name);
        var type = Conventions.MakeFieldType(property.PropertyType);
        ctx.Entity.Properties.AddDataHeader(name, title, type);
      }
      ctx.Entity.Properties.Add("_dataCount", properties.Length);
    }

    #endregion

    #region UpdateRowsHeaders

    private static Type GetDataType(RenderContext ctx)
    {
      return ctx.Data?.GetType() ?? ctx.Query.GetMethod("GetData").ReturnType;
    }

    private static void UpdateDataHeaders(RenderContext ctx)
    {
      var data = ctx.Query.Call("GetDataHeaders");
      if (data == null)
        return;

      var isStrings = typeof(IEnumerable<string>).IsAssignableFrom(data.GetType());
      var isEnumerable = typeof(IEnumerable).IsAssignableFrom(data.GetType());

      if (isStrings)
        UpdateDataHeaders_Custom_FromStrings(ctx);
      else if (isEnumerable)
        UpdateDataHeaders_Custom_FromEnumerable(ctx);
      else
        UpdateDataHeaders_Custom_FromObject(ctx);
    }

    private static void UpdateDataHeaders_Custom_FromStrings(RenderContext ctx)
    {
      var data = ctx.Query.Call<IEnumerable<string>>("GetDataHeaders");
      var headers = ctx.Entity.Properties.GetDataHeaders();

      var dataItems = data.GetEnumerator();
      var headerItems = headers.GetEnumerator();

      while (dataItems.MoveNext() && headerItems.MoveNext())
      {
        var title = Conventions.MakeFieldTitle(dataItems.Current);
        headerItems.Current.Title = title;
      }
    }

    private static void UpdateDataHeaders_Custom_FromEnumerable(RenderContext ctx)
    {
      var data = ctx.Query.Call<IEnumerable>("GetDataHeaders");
      var headers = ctx.Entity.Properties.GetDataHeaders();

      var dataItems = data.GetEnumerator();
      var headerItems = ctx.Entity.Properties.GetDataHeaders().GetEnumerator();

      while (dataItems.MoveNext() && headerItems.MoveNext())
      {
        object item = dataItems.Current;
        Header header = headerItems.Current;

        var name = item.Get<string>("Name");
        if (name != null)
        {
          name = Conventions.MakeFieldName(name);
          header = headers[name];
          if (header == null)
            continue; // A coluna referencia um campo nao encontrado nos registros
        }

        var oldHeaderName = header.Name;

        header.CopyFrom(item, CopyOptions.IgnoreNull);

        // corrigindo o nome da propriedade, caso tenha sido substituido
        header.Name = oldHeaderName;
      }
    }

    private static void UpdateDataHeaders_Custom_FromObject(RenderContext ctx)
    {
      var data = ctx.Query.Call("GetDataHeaders");
      var headers = ctx.Entity.Properties.GetDataHeaders();

      var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

      var properties = data.GetType().GetProperties(flags).Cast<PropertyInfo>().GetEnumerator();
      var headerItems = ctx.Entity.Properties.GetDataHeaders().GetEnumerator();

      while (properties.MoveNext() && headerItems.MoveNext())
      {
        Header header = headerItems.Current;

        var property = properties.Current;
        var item = property.GetValue(data);

        var name = Conventions.MakeFieldName(property.Name);
        header = headers[name];
        if (header == null)
          continue;

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