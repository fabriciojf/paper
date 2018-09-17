using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Paper.Media.Design;
using Paper.Media.Design.Mappings;
using Toolset;
using Toolset.Data;
using Toolset.Reflection;

namespace Paper.Media.Rendering.Papers
{
  public static class EntityActionConverter
  {
    public static EntityAction ConvertToEntityAction(object graph, PaperContext ctx)
    {
      var action = graph as EntityAction;
      if (action == null)
      {
        action = new EntityAction();
        action.Fields = new FieldCollection();

        var fields = graph as IEnumerable<Field>;
        if (fields != null)
        {
          action.Fields.AddRange(fields);
        }
        else
        {
          // Cada proprieade do objeto será mapeada como um campo de filtro
          foreach (var key in graph._GetPropertyNames())
          {
            var field = CreateField(graph, key, ctx);
            action.Fields.Add(field);
          }
        }
      }
      return action;
    }

    private static Field CreateField(object graph, string key, PaperContext ctx)
    {
      var field = new Field();
      var property = graph._GetPropertyInfo(key);

      //
      // Propriedades padrão
      //
      field.Name = Conventions.MakeName(property);
      field.Title = Conventions.MakeTitle(property);
      field.DataType = Conventions.MakeDataType(property);

      var isReadOnly = !property.CanWrite;
      if (isReadOnly)
      {
        field.AddReadOnly();
      }
      else
      {
        var isPrimitive =
          property.PropertyType.IsValueType
          && (Nullable.GetUnderlyingType(property.PropertyType) == null);
        if (isPrimitive)
        {
          field.AddRequired();
        }

        var isVar = typeof(IVar).IsAssignableFrom(property.PropertyType);
        if (isVar)
        {
          field.AddAllowMany();
          field.AddAllowRange();
        }

        var isList = typeof(IList).IsAssignableFrom(property.PropertyType);
        if (isList)
        {
          field.AddAllowMany();
        }
      }

      //
      // Propriedades personalizadas
      //
      var attributes = property._GetAttrs<FieldAttribute>();
      foreach (var attribute in attributes)
      {
        attribute.RenderField(field, property, graph, ctx);
      }

      //
      // Propriedades obrigatorias
      //
      field.Value = graph._Get(key);

      if (field.Name.StartsWith("_"))
      {
        field.AddHidden();
      }

      return field;
    }
  }
}
