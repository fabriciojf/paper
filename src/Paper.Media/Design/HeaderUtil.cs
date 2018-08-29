using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Design
{
  /// <summary>
  /// Utilitários para manipulação de cabeçalhos e colunas em entidads.
  /// </summary>
  internal class HeaderUtil
  {
    /// <summary>
    /// Adiciona informação de coluna em uma entidade.
    /// </summary>
    /// <param name="targetEntity">A entidade a ser modificada.</param>
    /// <param name="headerName">O nome da coluna.</param>
    /// <param name="headerTitle">Título da coluna.</param>
    /// <param name="headerDataType">Tipo do dado da coluna.</param>
    /// <param name="rel">A relação entre a coluna e a entidade.</param>
    /// <param name="inspectors">
    /// Métodos de inspeção do cabeçalho criado.
    /// Os métodos podem ser usados para modificar propriedades do cabeçalho.
    /// </param>
    public static Entity AddHeaderToEntity(
        Entity targetEntity
      , string targetPropertyName
      , string headerName
      , string headerTitle
      , string headerDataType
      , string rel
      , params Action<HeaderOptions>[] inspectors
      )
    {
      if (targetEntity.Entities == null)
      {
        targetEntity.Entities = new EntityCollection();
      }
      if (targetEntity.Properties == null)
      {
        targetEntity.Properties = new PropertyCollection();
      }

      // Adicionando o nome do campo à coleção de nomes de campos
      //
      targetEntity.AddProperty<List<CaseVariantString>>(targetPropertyName,
        list =>
        {
          if (!list.Any(x => x.Value.EqualsIgnoreCase(headerName)))
          {
            list.Add(headerName);
          }
        }
      );

      Entity header = (
        from e in targetEntity.Entities
        where e.Rel?.Contains(rel) == true
        where headerName.EqualsIgnoreCase(e.Properties?["Name"]?.Value?.ToString())
        select e
      ).FirstOrDefault();

      if (header == null)
      {
        header = new Entity();
        targetEntity.Entities.Add(header);
      }

      var title =
        headerTitle
        ?? header.Properties?["Title"]?.Value?.ToString()
        ?? headerName.ChangeCase(TextCase.ProperCase);

      var dataType =
        headerDataType
        ?? header.Properties?["DataType"]?.Value?.ToString()
        ?? DataTypeNames.Text;

      header
        .AddRel(rel)
        .AddClass(ClassNames.Header)
        .AddProperty("Name", (CaseVariantString)headerName)
        .AddProperty("Title", title)
        .AddProperty("DataType", dataType)
        ;

      if (headerName.StartsWith("_"))
      {
        header.AddProperty("Hidden", true);
      }

      if (inspectors.Any())
      {
        var options = new HeaderOptions(header);
        foreach (var inspector in inspectors)
        {
          inspector?.Invoke(options);
        }
      }

      return targetEntity;
    }
  }
}