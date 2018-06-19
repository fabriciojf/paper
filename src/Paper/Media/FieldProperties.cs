using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Paper.Media
{
  [DataContract(Namespace = Namespaces.Default)]
  [KnownType(typeof(CaseVariantString))]
  public class FieldProperties
  {
    /// <summary>
    /// Tamanho mínimo para um texto ou menor valor para um número.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public int? MinLength { get; set; }

    /// <summary>
    /// Tamanho máximo para um texto ou maior valor para um número.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Expressão regular para validação do conteúdo de um campo texto.
    /// A expressão deve seguir a mesma forma aplicada para restrição de
    /// texto no XSD (Esquema de XML).
    /// Referências:
    /// - https://www.regular-expressions.info/xml.html
    /// - http://www.xmlschemareference.com/regularExpression.html
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 25)]
    public string Pattern { get; set; }

    /// <summary>
    /// Ativa ou desativa a edição em múltiplas linhas, geralmente para campos texto.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 20)]
    public bool? Multiline { get; set; }

    /// <summary>
    /// Ativa ou desativa a múltipla seleção de valores para o campo.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 30)]
    public bool? AllowMany { get; set; }

    /// <summary>
    /// Ativa ou desativa o suporte a intervalo, na forma "{ min=x, max=y }".
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 5)]
    public bool? AllowRange { get; set; }

    /// <summary>
    /// Ativa ou desativa o suporte aos curingas "*", para indicar qualquer texto
    /// na posição, e "?", para indicar qualquer caracter na posição.
    /// </summary>
    [DataMember(EmitDefaultValue = false, Order = 5)]
    public bool? AllowWildcards { get; set; }
  }
}

