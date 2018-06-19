using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Paper.Media;
using Toolset;

namespace Paper.Media.Rendering
{
  public static class Conventions
  {
    public static string MakeFieldName(DataColumn col)
    {
      var colName = col.Caption ?? col.ColumnName ?? ("Col" + col.Ordinal);
      var name = colName.Split("|").First();
      return MakeFieldName(name);
    }

    public static string MakeFieldName(string name)
    {
      if (name.StartsWith("DF"))
      {
        name = name.Substring(2);
      }
      name = name.ChangeCase(TextCase.PascalCase);
      return name;
    }

    public static string MakeFieldTitle(DataColumn col)
    {
      var colName = col.Caption ?? col.ColumnName ?? ("Col" + col.Ordinal);
      var name = colName.Split("|").Last();
      return MakeFieldTitle(name);
    }

    public static string MakeFieldTitle(string name)
    {
      if (name.StartsWith("DF"))
      {
        name = name.Substring(2);
      }
      name = name.ChangeCase(TextCase.ProperCase | TextCase.PreserveSpecialChars);
      return name;
    }

    public static string MakeFieldType(DataColumn col)
    {
      return MakeFieldType(col.DataType);
    }

    public static string MakeFieldType(Type type)
    {
      return KnownFieldDataTypes.GetDataTypeName(type) ?? KnownFieldDataTypes.Text;
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
  }
}