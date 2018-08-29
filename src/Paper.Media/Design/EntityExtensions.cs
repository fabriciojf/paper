using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design
{
  /// <summary>
  /// Extensões de desenho de objetos Entity.
  /// </summary>
  public static class EntityExtensions
  {
    #region Básicos

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

    #endregion

    #region Operações especiais

    /// <summary>
    /// Resolve os links relativos segundo o padrão de URLs do Paper:
    /// -   /Path, corresponde a um caminho relativo à API
    ///     Como em:
    ///         http://localhost/Api/1/Path
    /// -   ^/Path, corresponde a um caminho relativo à raiz da URI
    ///     Como em:
    ///         http://localhost/Path
    /// -   ./Path ou ../Path, corresponde a um caminho relativo à URI atual
    ///     Como em:
    ///         http://localhost/Api/1/Meu/Site/Path
    /// </summary>
    /// <param name="entity">A entidade a ser modificada.</param>
    /// <param name="requestUri">A URI de requisição da entidade.</param>
    /// <param name="apiPath">
    /// O caminho considerado como caminho da API.
    /// Geralmente "/Api/VERSAO", sendo versão o número de versão de API do Paper.
    /// Por padrão: "/Api/1"
    /// </param>
    /// <returns>A própria instância da entidade modificada.</returns>
    public static Entity ResolveLinks(this Entity entity, string requestUri, string apiPath = "/Api/1")
    {
      var route = new Route(requestUri).UnsetAllArgsExcept("f", "in", "out");

      var entities = DescendantsAndSelf(entity);
      var links = entities.Select(x => x.Links).NonNull().SelectMany();
      var actions = entities.Select(x => x.Actions).NonNull().SelectMany();

      foreach (var link in links)
      {
        link.Href = ResolveLink(link.Href, route, apiPath);
      }

      foreach (var action in actions)
      {
        action.Href = ResolveLink(action.Href, route, apiPath);
      }

      return entity;
    }

    /// <summary>
    /// Resolve os links relativos segundo o padrão de URLs do Paper:
    /// -   /Path, corresponde a um caminho relativo à API
    ///     Como em:
    ///         http://localhost/Api/1/Path
    /// -   ^/Path, corresponde a um caminho relativo à raiz da URI
    ///     Como em:
    ///         http://localhost/Path
    /// -   ./Path ou ../Path, corresponde a um caminho relativo à URI atual
    ///     Como em:
    ///         http://localhost/Api/1/Meu/Site/Path
    /// </summary>
    /// <param name="href">A URI a ser resolvida.</param>
    /// <param name="currentUri">A URI representando a rota corrente.</param>
    /// <param name="apiPath">
    /// O caminho considerado como caminho da API.
    /// Geralmente "/Api/VERSAO", sendo versão o número de versão de API do Paper.
    /// Por padrão: "/Api/1"
    /// </param>
    /// <returns>A URI resolvida.</returns>
    private static string ResolveLink(string href, Route currentUri, string apiPath)
    {
      if (href.StartsWith("^/"))
      {
        href = currentUri.Combine(href.Substring(1));
      }
      else if (href.StartsWith("/"))
      {
        href = currentUri.Combine(apiPath).Append(href);
      }
      else if (href == ""
            || href.StartsWith(".")
            || href.StartsWith("?"))
      {
        href = currentUri.Combine(href);
      }
      return href;
    }

    /// <summary>
    /// Enumera recursivamente todos os filhos da entidade.
    /// </summary>
    /// <param name="entity">A entidade analisada.</param>
    /// <returns>Todos os filhos da entidade recursivamente.</returns>
    public static IEnumerable<Entity> Descendants(this Entity entity)
    {
      return
        entity?.Entities?.SelectMany(e => Descendants(e).Append(e))
        ?? Enumerable.Empty<Entity>();
    }

    /// <summary>
    /// Enumera recursivamente todos os filhos da entidade e a própria entidade.
    /// </summary>
    /// <param name="entity">A entidade analisada.</param>
    /// <returns>Todos os filhos da entidade recursivamente e a própria entidade.</returns>
    public static IEnumerable<Entity> DescendantsAndSelf(this Entity entity)
    {
      return
        entity?.Entities?.SelectMany(e => DescendantsAndSelf(e)).Append(entity)
        ?? entity.AsSingle();
    }

    #endregion

  }
}
