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
  /// Extensões de desenho de ações de objetos Entity.
  /// </summary>
  public static class MediaObjectExtensions
  {
    /// <summary>
    /// Define o título do link.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="title">O novo título do link.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddTitle<TMediaObject>(this TMediaObject target, string title)
      where TMediaObject : IMediaObject
    {
      target.Title = title;
      return target;
    }

    /// <summary>
    /// Constrói um título para a entidade a partir do tipo indicado.
    /// O título é lido do atributo de classe [DisplayName], caso não exista,
    /// o título é construído a partir do próprio nome do tipo.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="baseType">
    /// O tipo que será usado como base para definição do título.
    /// </param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddTitle<TMediaObject>(this TMediaObject target, Type baseType)
      where TMediaObject : IMediaObject
    {
      var attribute =
        baseType
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();

      target.Title =
        attribute?.DisplayName
        ?? baseType.Name.ChangeCase(TextCase.ProperCase);

      return target;
    }

    /// <summary>
    /// Constrói um título para a entidade a partir do tipo indicado.
    /// O título é lido do atributo de classe [DisplayName], caso não exista,
    /// o título é construído a partir do próprio nome do tipo.
    /// </summary>
    /// <typeparam name="TClass">
    /// O tipo que será usado como base para definição do título.
    /// </typeparam>
    /// <param name="link">O link a ser modificado.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddTitle<TMediaObject, TClass>(this TMediaObject link)
      where TMediaObject : IMediaObject
    {
      return AddTitle(link, typeof(TClass));
    }

    /// <summary>
    /// Adiciona as classes indicadas à entidade.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="className">O nome de uma classe.</param>
    /// <param name="otherClassNames">Os nomes de outras classes.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddClass<TMediaObject>(this TMediaObject target, string className, params string[] otherClassNames)
      where TMediaObject : IMediaObject
    {
      if (target.Class == null)
      {
        target.Class = new NameCollection();
      }
      target.Class.Add(className);
      target.Class.AddMany(otherClassNames);
      return target;
    }

    /// <summary>
    /// Adiciona as classes indicadas à entidade.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="className">O nome de uma classe.</param>
    /// <param name="otherClassNames">Os nomes de outras classes.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddClass<TMediaObject>(this TMediaObject target, Class className, params Class[] otherClassNames)
      where TMediaObject : IMediaObject
    {
      if (target.Class == null)
      {
        target.Class = new NameCollection();
      }
      target.Class.Add(className.GetName());
      target.Class.AddMany(otherClassNames.Select(ClassExtensions.GetName));
      return target;
    }

    /// <summary>
    /// Infere nomes de classes para a entidade baseado nos tipos indicados.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="type">Um tipo para inferência de nome de classe.</param>
    /// <param name="otherTypes">Outros tipos para inferência de nomes de classe.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddClass<TMediaObject>(this TMediaObject target, Type type, params Type[] otherTypes)
      where TMediaObject : IMediaObject
    {
      if (target.Class == null)
      {
        target.Class = new NameCollection();
      }
      target.Class.Add(DataTypeNames.GetDataTypeName(type));
      target.Class.AddMany(
        otherTypes.Select(x => DataTypeNames.GetDataTypeName(x))
      );
      return target;
    }

    /// <summary>
    /// Infere um nome de classe a partir do tipo indicado.
    /// </summary>
    /// <typeparam name="TClass">Tipo base para inferência do nome de classe.</typeparam>
    /// <param name="target">O link a ser modificado.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddClass<TMediaObject, TClass>(this TMediaObject target)
      where TMediaObject : IMediaObject
    {
      if (target.Class == null)
      {
        target.Class = new NameCollection();
      }
      target.Class.Add(DataTypeNames.GetDataTypeName(typeof(TClass)));
      return target;
    }

    /// <summary>
    /// Adiciona as relações indicadas à entidade.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="rel">O nome de uma relação.</param>
    /// <param name="otherRels">Nomes de outras relações.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddRel<TMediaObject>(this TMediaObject target, string rel, params string[] otherRels)
      where TMediaObject : IMediaObject
    {
      if (target.Rel == null)
      {
        target.Rel = new NameCollection();
      }
      target.Rel.Add(rel);
      target.Rel.AddMany(otherRels);
      return target;
    }

    /// <summary>
    /// Adiciona as relações indicadas à entidade.
    /// </summary>
    /// <param name="target">O link a ser modificado.</param>
    /// <param name="rel">O nome de uma relação.</param>
    /// <param name="otherRels">Nomes de outras relações.</param>
    /// <returns>A própria instância do link modificado.</returns>
    public static TMediaObject AddRel<TMediaObject>(this TMediaObject target, Rel rel, params Rel[] otherRels)
      where TMediaObject : IMediaObject
    {
      if (target.Rel == null)
      {
        target.Rel = new NameCollection();
      }
      target.Rel.Add(rel.GetName());
      target.Rel.AddMany(otherRels.Select(RelExtensions.GetName));
      return target;
    }
  }
}