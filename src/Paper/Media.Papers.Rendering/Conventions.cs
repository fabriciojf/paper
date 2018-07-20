using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Paper.Media;
using Toolset;

namespace Paper.Media.Papers.Rendering
{
  public static class Conventions
  {
    public static string MakeFieldName(DataColumn column)
    {
      var name = column.Caption ?? column.ColumnName ?? ("Col" + column.Ordinal);
      return MakeFieldName(name);
    }

    public static string MakeFieldName(PropertyInfo property)
    {
      return MakeFieldName(property.Name);
    }

    public static string MakeFieldName(string name)
    {
      name = name.Split("|").First();
      if (name.StartsWithIgnoreCase("DF"))
      {
        name = name.Substring(2);
      }
      name = name.ChangeCase(TextCase.PascalCase);
      return name;
    }

    public static string MakeFieldTitle(DataColumn column)
    {
      var name = column.Caption ?? column.ColumnName ?? ("Col" + column.Ordinal);
      return MakeFieldTitle(name);
    }

    public static string MakeFieldTitle(PropertyInfo property)
    {
      var attr = 
        property
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();

      var name = attr?.DisplayName ?? property.Name;
      return MakeFieldTitle(name);
    }

    public static string MakeFieldTitle(string name)
    {
      if (name.Contains("|"))
      {
        name = name.Split("|").Last();
      }
      if (name.StartsWithIgnoreCase("DF"))
      {
        name = name.Substring(2);
      }
      
      name = name.ChangeCase(TextCase.ProperCase | TextCase.PreserveSpecialCharacters);
      return name;
    }

    public static string MakeFieldType(DataColumn col)
    {
      return DataTypeNames.GetDataTypeName(col.DataType) ?? DataTypeNames.Text;
    }

    public static string MakeFieldType(Type type)
    {
      return DataTypeNames.GetDataTypeName(type) ?? DataTypeNames.Text;
    }

    public static string MakeClassName(object typeOrObject)
    {
      if (typeOrObject == null)
        return null;

      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      if (type.FullName.Contains("AnonymousType"))
        return null;

      return type.FullName;
    }

    public static string MakeClassTitle(object typeOrObject)
    {
      if (typeOrObject == null)
        return null;

      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      if (type.FullName.Contains("AnonymousType"))
        return null;

      return type.Name.Replace("Paper", "").ChangeCase(TextCase.ProperCase);
    }
  }
}