﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Design
{
  /// <summary>
  /// Extensões de desenho de objetos Entity.
  /// </summary>
  public static class EntityExtensions
  {
    /// <summary>
    /// Define o título da entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="title">O novo título da entidade.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddTitle(this Entity entity, string title)
    {
      entity.Title = title;
      return entity;
    }

    /// <summary>
    /// Constrói um título para a entidade a partir do tipo indicado.
    /// O título é lido do atributo de classe [DisplayName], caso não exista,
    /// o título é construído a partir do próprio nome do tipo.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="baseType">
    /// O tipo que será usado como base para definição do título.
    /// </param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddTitle(this Entity entity, Type baseType)
    {
      var attribute =
        baseType
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();

      entity.Title =
        attribute?.DisplayName
        ?? baseType.Name.ChangeCase(TextCase.ProperCase);

      return entity;
    }

    /// <summary>
    /// Constrói um título para a entidade a partir do tipo indicado.
    /// O título é lido do atributo de classe [DisplayName], caso não exista,
    /// o título é construído a partir do próprio nome do tipo.
    /// </summary>
    /// <typeparam name="T">
    /// O tipo que será usado como base para definição do título.
    /// </typeparam>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddTitle<T>(this Entity entity)
    {
      return AddTitle(entity, typeof(T));
    }

    /// <summary>
    /// Adiciona as classes indicadas à entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="className">O nome de uma classe.</param>
    /// <param name="otherClassNames">Os nomes de outras classes.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddClass(this Entity entity, string className, params string[] otherClassNames)
    {
      if (entity.Class == null)
      {
        entity.Class = new NameCollection();
      }
      entity.Class.Add(className);
      entity.Class.AddMany(otherClassNames);
      return entity;
    }

    /// <summary>
    /// Infere nomes de classes para a entidade baseado nos tipos indicados.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="type">Um tipo para inferência de nome de classe.</param>
    /// <param name="otherTypes">Outros tipos para inferência de nomes de classe.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddClass(this Entity entity, Type type, params Type[] otherTypes)
    {
      if (entity.Class == null)
      {
        entity.Class = new NameCollection();
      }
      entity.Class.Add(DataTypeNames.GetDataTypeName(type));
      entity.Class.AddMany(
        otherTypes.Select(x => DataTypeNames.GetDataTypeName(x))
      );
      return entity;
    }

    /// <summary>
    /// Infere um nome de classe a partir do tipo indicado.
    /// </summary>
    /// <typeparam name="T">Tipo base para inferência do nome de classe.</typeparam>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddClass<T>(this Entity entity)
    {
      if (entity.Class == null)
      {
        entity.Class = new NameCollection();
      }
      entity.Class.Add(DataTypeNames.GetDataTypeName(typeof(T)));
      return entity;
    }

    /// <summary>
    /// Adiciona as relações indicadas à entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="rel">O nome de uma relação.</param>
    /// <param name="otherRels">Nomes de outras relações.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddRel(this Entity entity, string rel, params string[] otherRels)
    {
      if (entity.Rel == null)
      {
        entity.Rel = new NameCollection();
      }
      entity.Rel.Add(rel);
      entity.Rel.AddMany(otherRels);
      return entity;
    }

    /// <summary>
    /// Adiciona as relações indicadas à entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="rel">O nome de uma relação.</param>
    /// <param name="otherRels">Nomes de outras relações.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddRel(this Entity entity, Rel rel, params Rel[] otherRels)
    {
      if (entity.Rel == null)
      {
        entity.Rel = new NameCollection();
      }
      entity.Rel.Add(rel.GetName());
      entity.Rel.AddMany(otherRels.Select(RelExtensions.GetName));
      return entity;
    }

    /// <summary>
    /// Adiciona uma entidade filha à coleção de entidades filhas da entidade indicada.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="child">A entidade a ser adicionada à coleção de entidades filhas.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddEntity(this Entity entity, Entity child)
    {
      if (entity.Entities == null)
      {
        entity.Entities = new EntityCollection();
      }
      entity.Entities.Add(child);
      return entity;
    }

    /// <summary>
    /// Adiciona uma entidade filha à coleção de entidades filhas da entidade indicada.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="rel">O relacionamento entre a entidade filha e a entidade principal.</param>
    /// <param name="builder">Um método de construção da entidade filha.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddEntity(this Entity entity, Rel rel, Action<Entity> builder)
    {
      var child = new Entity();
      child.Rel = new NameCollection();
      child.Rel.Add(rel.GetName());
      builder.Invoke(child);

      if (entity.Entities == null)
      {
        entity.Entities = new EntityCollection();
      }
      entity.Entities.Add(child);

      return entity;
    }

    /// <summary>
    /// Adiciona uma entidade filha à coleção de entidades filhas da entidade indicada.
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="rel">O relacionamento entre a entidade filha e a entidade principal.</param>
    /// <param name="builder">Um método de construção da entidade filha.</param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity AddEntity(this Entity entity, string rel, Action<Entity> builder)
    {
      var child = new Entity();
      child.Rel = new NameCollection();
      child.Rel.Add(rel);
      builder.Invoke(child);

      if (entity.Entities == null)
      {
        entity.Entities = new EntityCollection();
      }
      entity.Entities.Add(child);

      return entity;
    }
  }
}