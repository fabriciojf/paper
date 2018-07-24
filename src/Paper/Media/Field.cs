using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Paper.Media.Serialization;
using Toolset;

namespace Paper.Media
{
  [DataContract(Namespace = Namespaces.Default)]
  [KnownType(typeof(FieldValueCollection))]
  [KnownType(typeof(CaseVariantString))]
  public class Field : IMediaObject
  {
    private string _type;
    private string _dataType;
    private string _title;

    /// <summary>
    /// Tipo de objeto do campo.
    /// 
    /// Mais de um tipo pode ser indicado para
    /// campos com mais de um comportamento.
    /// 
    /// A ordem dos tipos importa. O aplicativo cliente pode
    /// optar por refletir apenas o primeiro tipo definido, ou o
    /// formato de serialização pode não suportar mais de um tipo.
    /// Sempre defina o tipo primário antes dos tipos alternativos.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 3)]
    public virtual NameCollection Class { get; set; }

    /// <summary>
    /// Nome do campo.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 7)]
    [CaseVariantString]
    public virtual string Name { get; set; }

    /// <summary>
    /// Tipo do componente de edição do campo.
    /// 
    /// É aceito qualquer um dos tipos convencionados para o HTML5:
    /// - hidden
    /// - text
    /// - search
    /// - tel
    /// - url
    /// - email
    /// - password
    /// - datetime
    /// - date
    /// - month
    /// - week
    /// - time
    /// - datetime-local
    /// - number
    /// - range
    /// - color
    /// - checkbox
    /// - radio
    /// - file
    /// 
    /// A lista completa está definida na classe FieldTypeNames.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public virtual string Type
    {
      get { return _type ?? FieldTypeNames.GetFieldTypeFromDataType(DataType); }
      set { _type = value; }
    }

    /// <summary>
    /// Tipo do valor do campo.
    /// 
    /// Tipos aceitos:
    /// - text (string)
    /// - bit (bool, boolean)
    /// - number (int, long)
    /// - decimal (double, float)
    /// - date
    /// - time
    /// - datetime
    /// 
    /// O texto em parêntesis representa um nome alternativo, ou apelido, para o tipo.
    /// 
    /// A lista completa está definida na classe FieldDataTypeNames.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 15)]
    public virtual string DataType
    {
      get { return _dataType ?? DataTypeNames.Text; }
      set { _dataType = value; }
    }

    /// <summary>
    /// Título do campo. Opcional.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public virtual string Title
    {
      get => _title ?? (Name ?? "").ChangeCase(TextCase.ProperCase);
      set => _title = value;
    }

    /// <summary>
    /// Determina o relacionamento do campo com a sua ação.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 25)]
    public virtual NameCollection Rel { get; set; }

    /// <summary>
    /// Nome da categoria do campo para criação de agrupamentos.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 30)]
    public virtual string Category { get; set; }

    /// <summary>
    /// Url do provedor de dados do campo.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 35)]
    public virtual string Provider { get; set; }

    /// <summary>
    /// Valor do campo. Opcional.
    /// Um valor pode ser um dos tipos básicos do C# ou uma coleção
    /// FieldValueCollection.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 40)]
    public virtual object Value { get; set; }

    /// <summary>
    /// Ativa ou desativa a obrigatoriedade de preenchimento do campo.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 70)]
    public virtual bool? Required { get; set; }

    /// <summary>
    /// Torna o campo editável ou somente leitura.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 80)]
    public virtual bool? ReadOnly { get; set; }

    /// <summary>
    /// Tamanho mínimo para um texto ou menor valor para um número.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 100)]
    public virtual int? MinLength { get; set; }

    /// <summary>
    /// Tamanho máximo para um texto ou maior valor para um número.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 200)]
    public virtual int? MaxLength { get; set; }

    /// <summary>
    /// Expressão regular para validação do conteúdo de um campo texto.
    /// A expressão deve seguir a mesma forma aplicada para restrição de
    /// texto no XSD (Esquema de XML).
    /// Referências:
    /// - https://www.regular-expressions.info/xml.html
    /// - http://www.xmlschemareference.com/regularExpression.html
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 250)]
    public virtual string Pattern { get; set; }

    /// <summary>
    /// Ativa ou desativa a edição em múltiplas linhas, geralmente para campos texto.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 200)]
    public virtual bool? Multiline { get; set; }

    /// <summary>
    /// Ativa ou desativa a múltipla seleção de valores para o campo.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 300)]
    public virtual bool? AllowMany { get; set; }

    /// <summary>
    /// Ativa ou desativa o suporte a intervalo, na forma "{ min=x, max=y }".
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 400)]
    public virtual bool? AllowRange { get; set; }

    /// <summary>
    /// Ativa ou desativa o suporte aos curingas "*", para indicar qualquer texto
    /// na posição, e "?", para indicar qualquer caracter na posição.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 500)]
    public virtual bool? AllowWildcards { get; set; }
  }
}

