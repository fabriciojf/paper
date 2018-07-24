using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Paper.Media;
using Toolset;

namespace Paper.Media.Design.Papers.Rendering
{
  public static class Conventions
  {
    public static string MakeName(DataColumn column)
    {
      var name = column.Caption ?? column.ColumnName ?? ("Col" + column.Ordinal);
      return MakeName(name);
    }

    public static string MakeName(PropertyInfo property)
    {
      return MakeName(property.Name);
    }

    public static string MakeName(string name)
    {
      name = name.Split("|").First();
      if (name.StartsWithIgnoreCase("DF"))
      {
        name = name.Substring(2);
      }
      name = name.ChangeCase(TextCase.PascalCase);
      return name;
    }

    public static string MakeName(object typeOrObject)
    {
      if (typeOrObject == null)
        return null;

      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      if (type.FullName.Contains("AnonymousType"))
        return null;

      return type.FullName;
    }

    public static string MakeTitle(DataColumn column)
    {
      var name = column.Caption ?? column.ColumnName ?? ("Col" + column.Ordinal);
      return MakeTitle(name);
    }

    public static string MakeTitle(PropertyInfo property)
    {
      var attr = 
        property
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();

      var name = attr?.DisplayName ?? property.Name;
      return MakeTitle(name);
    }

    public static string MakeTitle(string name)
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

    public static string MakeTitle(object typeOrObject)
    {
      if (typeOrObject == null)
        return null;

      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      if (type.FullName.Contains("AnonymousType"))
        return null;

      var name = type.Name
          .Replace("Paper", "")
          .Replace("Entity", "")
          .ChangeCase(TextCase.ProperCase);
      return name;
    }

    public static string MakeDataType(DataColumn col)
    {
      return DataTypeNames.GetDataTypeName(col.DataType) ?? DataTypeNames.Text;
    }

    public static string MakeDataType(Type type)
    {
      return DataTypeNames.GetDataTypeName(type) ?? DataTypeNames.Text;
    }

    public static string MakeDataType(PropertyInfo property)
    {
      return DataTypeNames.GetDataTypeName(property.PropertyType) ?? DataTypeNames.Text;
    }
  }
}