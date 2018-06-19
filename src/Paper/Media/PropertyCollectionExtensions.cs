using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Toolset.Collections;
using Toolset;
using System.ComponentModel;

namespace Paper.Media
{
  public static class PropertyCollectionExtensions
  {
    #region Extensões para manipulação de itens

    public static void AddFromGraph(this PropertyCollection properties, object graph)
    {
      Property[] items;

      var type = graph.GetType();
      var isDataContract =
        type
          .GetCustomAttributes(true)
          .OfType<DataContractAttribute>()
          .Any();

      if (isDataContract)
      {
        items = (
          from prop in type.GetProperties()
          from member in prop.GetCustomAttributes(true).OfType<DataMemberAttribute>()
          orderby member.Order
          let value = prop.GetValue(graph)
          where member.EmitDefaultValue || (value != null)
          select new Property
          {
            Name = member.Name ?? prop.Name,
            Value = value
          }
        ).ToArray();
      }
      else
      {
        items = (
          from prop in type.GetProperties()
          let value = prop.GetValue(graph)
          select new Property
          {
            Name = prop.Name,
            Value = value
          }
        ).ToArray();
      }

      properties.AddMany(items);
    }

    #endregion

    #region Extensões para manipulação de cabeçalhos de propriedades

    public static HeaderCollection GetDataHeaders(this PropertyCollection properties)
    {
      HeaderCollection collection = properties[HeaderCollection.DataHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.DataHeadersName, collection);
      }
      return collection;
    }

    public static void AddDataHeaders(this PropertyCollection properties, HeaderCollection headers)
    {
      properties.Add(HeaderCollection.DataHeadersName, headers);
    }

    public static HeaderCollection AddDataHeaders(this PropertyCollection properties, IEnumerable<Header> headers)
    {
      HeaderCollection collection = properties[HeaderCollection.DataHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.DataHeadersName, collection);
      }
      collection.AddRange(headers);
      return collection;
    }

    public static HeaderCollection AddDataHeadersFromGraph<T>(this PropertyCollection properties)
    {
      return AddDataHeadersFromGraph(properties, typeof(T));
    }

    public static HeaderCollection AddDataHeadersFromGraph(this PropertyCollection properties, object graphOrType)
    {
      Header[] headers;

      var type = (graphOrType is Type) ? (Type)graphOrType : graphOrType.GetType();
      var isDataContract =
        type
          .GetCustomAttributes(true)
          .OfType<DataContractAttribute>()
          .Any();

      if (isDataContract)
      {
        headers = (
          from prop in type.GetProperties()
          from member in prop.GetCustomAttributes(true).OfType<DataMemberAttribute>()
          orderby member.Order
          select new Header
          {
            Name = member.Name ?? prop.Name,
            Type = KnownFieldDataTypes.GetDataTypeName(prop.PropertyType),
            Title = (member.Name ?? prop.Name).ChangeCase(TextCase.ProperCase),
          }
        ).ToArray();
      }
      else
      {
        headers = (
          from prop in type.GetProperties()
          let attribute =
            prop.GetCustomAttributes(true)
                .OfType<DisplayNameAttribute>()
                .FirstOrDefault()
          select new Header
          {
            Name = prop.Name,
            Type = KnownFieldDataTypes.GetDataTypeName(prop.PropertyType),
            Title = (attribute?.DisplayName ?? prop.Name).ChangeCase(TextCase.ProperCase),
          }
        ).ToArray();
      }

      return AddDataHeaders(properties, headers);
    }

    public static Header AddDataHeader(this PropertyCollection properties, string name)
    {
      HeaderCollection collection = properties[HeaderCollection.DataHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.DataHeadersName, collection);
      }
      var header = collection.Add(name);
      return header;
    }

    public static Header AddDataHeader(this PropertyCollection properties, string name, string title, string type)
    {
      HeaderCollection collection = properties[HeaderCollection.DataHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.DataHeadersName, collection);
      }
      var header = collection.Add(name, title, type);
      return header;
    }

    #endregion

    #region Extensões para manipulação de cabeçalhos de lista

    public static HeaderCollection GetRowsHeaders(this PropertyCollection properties)
    {
      HeaderCollection collection = properties[HeaderCollection.RowsHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.RowsHeadersName, collection);
      }
      return collection;
    }

    public static void AddRowsHeaders(this PropertyCollection properties, HeaderCollection headers)
    {
      properties.Add(HeaderCollection.RowsHeadersName, headers);
    }

    public static HeaderCollection AddRowsHeaders(this PropertyCollection properties, IEnumerable<Header> headers)
    {
      HeaderCollection collection = properties[HeaderCollection.RowsHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.RowsHeadersName, collection);
      }
      collection.AddRange(headers);
      return collection;
    }

    public static HeaderCollection AddRowsHeadersFromGraph<T>(this PropertyCollection properties)
    {
      return AddRowsHeadersFromGraph(properties, typeof(T));
    }

    public static HeaderCollection AddRowsHeadersFromGraph(this PropertyCollection properties, object graphOrType)
    {
      Header[] headers;

      var type = (graphOrType is Type) ? (Type)graphOrType : graphOrType.GetType();
      var isDataContract =
        type
          .GetCustomAttributes(true)
          .OfType<DataContractAttribute>()
          .Any();

      if (isDataContract)
      {
        headers = (
          from prop in type.GetProperties()
          from member in prop.GetCustomAttributes(true).OfType<DataMemberAttribute>()
          orderby member.Order
          select new Header
          {
            Name = member.Name ?? prop.Name,
            Type = KnownFieldDataTypes.GetDataTypeName(prop.PropertyType),
            Title = (member.Name ?? prop.Name).ChangeCase(TextCase.ProperCase),
          }
        ).ToArray();
      }
      else
      {
        headers = (
          from prop in type.GetProperties()
          let attribute =
            prop.GetCustomAttributes(true)
                .OfType<DisplayNameAttribute>()
                .FirstOrDefault()
          select new Header
          {
            Name = prop.Name,
            Type = KnownFieldDataTypes.GetDataTypeName(prop.PropertyType),
            Title = (attribute?.DisplayName ?? prop.Name).ChangeCase(TextCase.ProperCase),
          }
        ).ToArray();
      }

      return AddRowsHeaders(properties, headers);
    }

    public static Header AddRowsHeader(this PropertyCollection properties, string name)
    {
      HeaderCollection collection = properties[HeaderCollection.RowsHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.RowsHeadersName, collection);
      }
      var header = collection.Add(name);
      return header;
    }

    public static Header AddRowsHeader(this PropertyCollection properties, string name, string title, string type)
    {
      HeaderCollection collection = properties[HeaderCollection.RowsHeadersName]?.Value as HeaderCollection;
      if (collection == null)
      {
        collection = new HeaderCollection();
        properties.Add(HeaderCollection.RowsHeadersName, collection);
      }
      var header = collection.Add(name, title, type);
      return header;
    }

    #endregion
  }
}