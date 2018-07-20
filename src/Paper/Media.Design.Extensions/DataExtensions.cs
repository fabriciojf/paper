using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design.Extensions
{
  /// <summary>
  /// Extensões de desenho de dados que acompanham a entidade.
  /// 
  /// -   Os dados são representados como propriedades da entidade.
  /// -   Entidades que carregam dados recebem a classe <see cref="ClassNames.Data"/>.
  /// -   Para cada coluna de dado uma entidade filha correspondente é acrescentada.
  ///     -   A entidade filha contém a propriedade "Name" para vinculação do nome da coluna.
  ///     -   A entidade filha contém a classe <see cref="ClassNames.Header"/>.
  ///     -   A entidade filha contém a relação <see cref="RelNames.DataHeader"/>.
  /// </summary>
  public static class DataExtensions
  {
    public const string HeaderNamesProperty = "__DataHeaders";

    #region ForEach...

    /// <summary>
    /// Itera sob a coleção de cabeçalhos de dados na entidade.
    /// </summary>
    /// <param name="entity">A entidade inspecionada.</param>
    /// <param name="inspection">A função de inspeção do item.</param>
    /// <returns>A própria instância da entidade inspecionada.</returns>
    public static Entity ForEachDataHeader(this Entity entity, Action<Entity, HeaderInfo> inspection)
    {
      if (entity.Entities == null)
        return entity;

      var headers =
        from child in entity.Entities
        where child.Class?.Contains(ClassNames.Header) == true
           && child.Rel?.Contains(RelNames.DataHeader) == true
        select child;

      headers.ForEach(child =>
      {
        var properties =
          child.Properties
          ?? (child.Properties = new PropertyCollection());
        inspection(child, new HeaderInfo(properties));
      });

      return entity;
    }

    #endregion

    #region AddData

    /// <summary>
    /// Adiciona as propriedades do objeto indicado como dados na entidade indicada.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="data">Os dados adicionados à entidade.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddData(this Entity entity, object data)
    {
      if (entity.Entities == null)
      {
        entity.Entities = new EntityCollection();
      }
      if (entity.Properties == null)
      {
        entity.Properties = new PropertyCollection();
      }

      entity.AddClass(Class.Data);
      entity.AddDataHeadersFrom(data);
      entity.AddProperties(data);

      return entity;
    }

    #endregion

    #region AddDataHeader

    /// <summary>
    /// Adiciona informações sobre campos.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="headers">Os dados adicionados à entidade.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeaders(this Entity entity, IEnumerable<HeaderInfo> headers)
    {
      if (headers != null)
      {
        foreach (var header in headers)
        {
          HeaderUtil.AddHeaderToEntity(
              entity
            , HeaderNamesProperty
            , header.Name
            , header.Title
            , header.DataType
            , RelNames.DataHeader
            , options => header.CopyToHeaderOptions(options)
          );
        }
      }
      return entity;
    }

    /// <summary>
    /// Adiciona informações sobre um campo.
    /// </summary>
    /// <typeparam name="T">Um tipo para inferência dos campos.</typeparam>
    /// <param name="entity">A entidade modificada.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeadersFrom<T>(this Entity entity, Action<HeaderOptions> builder = null)
    {
      var properties = Property.UnwrapPropertyInfo(typeof(T));
      foreach (var property in properties)
      {
        HeaderUtil.AddHeaderToEntity(
            entity
          , HeaderNamesProperty
          , property.Name
          , property.Title
          , DataTypeNames.GetDataTypeName(property.Type)
          , RelNames.DataHeader
          , builder
        );
      }
      return entity;
    }

    /// <summary>
    /// Adiciona informações sobre um campo.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="typeOrInstance">Um tipo ou instância para inferência dos campos.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeadersFrom(this Entity entity, object typeOrInstance, Action<HeaderOptions> builder = null)
    {
      var properties = Property.UnwrapPropertyInfo(typeOrInstance);
      foreach (var property in properties)
      {
        HeaderUtil.AddHeaderToEntity(
            entity
          , HeaderNamesProperty
          , property.Name
          , property.Title
          , DataTypeNames.GetDataTypeName(property.Type)
          , RelNames.DataHeader
          , builder
        );
      }
      return entity;
    }

    /// <summary>
    /// Adiciona informações sobre um campo.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="name">Os dados adicionados à entidade.</param>
    /// <param name="builder">Construtor do cabeçalho.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeader(this Entity entity, string name, Action<HeaderOptions> builder = null)
    {
      HeaderUtil.AddHeaderToEntity(
          entity
        , HeaderNamesProperty
        , name
        , null
        , null
        , RelNames.DataHeader
        , builder
      );
      return entity;
    }

    /// <summary>
    /// Adiciona informações sobre um campo.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="name">Os dados adicionados à entidade.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="dataType">Tipo do dado do campo.</param>
    /// <param name="hidden">Marcao ou desmarca o campo como oculto.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeader(this Entity entity, string name, string title = null, string dataType = null, bool hidden = false)
    {
      HeaderUtil.AddHeaderToEntity(
          entity
        , HeaderNamesProperty
        , name
        , title
        , dataType
        , RelNames.DataHeader
        , opt => opt.AddHidden(hidden)
      );
      return entity;
    }

    /// <summary>
    /// Adiciona informações sobre um campo.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="header">Informações cobre o campo.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeader(this Entity entity, HeaderInfo header)
    {
      HeaderUtil.AddHeaderToEntity(
          entity
        , HeaderNamesProperty
        , header.Name
        , header.Title
        , header.DataType
        , RelNames.DataHeader
        , options => header.CopyToHeaderOptions(options)
      );
      return entity;
    }

    /// <summary>
    /// Adiciona informações sobre um campo oculto.
    /// </summary>
    /// <param name="entity">A entidade modificada.</param>
    /// <param name="name">Os dados adicionados à entidade.</param>
    /// <param name="title">Título do campo.</param>
    /// <param name="dataType">Tipo do dado do campo.</param>
    /// <returns>A própria entidade modificada.</returns>
    public static Entity AddDataHeaderHidden(this Entity entity, string name, string title = null, string dataType = null)
    {
      HeaderUtil.AddHeaderToEntity(
          entity
        , HeaderNamesProperty
        , name
        , title
        , dataType
        , RelNames.DataHeader
        , opt => opt.AddHidden()
      );
      return entity;
    }

    #endregion
  }
}
