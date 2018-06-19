using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Design.Widgets
{
  public class TextWidget : Widget, IHasLength, IHasMany, IHasOptions<string>
  {
    public TextWidget(string name)
      : base(name, KnownFieldDataTypes.Text)
    {
    }

    /// <summary>
    /// Tamanho mínimo para um texto ou menor valor para um número.
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Tamanho máximo para um texto ou maior valor para um número.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Expressão regular para validação do conteúdo de um campo texto.
    /// A expressão deve seguir a mesma forma aplicada para restrição de
    /// texto no XSD (Esquema de XML).
    /// Referências:
    /// - https://www.regular-expressions.info/xml.html
    /// - http://www.xmlschemareference.com/regularExpression.html
    /// </summary>
    public string Pattern { get; set; }

    /// <summary>
    /// Ativa ou desativa a múltipla seleção de valores para o campo.
    /// </summary>
    public bool? AllowMany { get; set; }

    /// <summary>
    /// Ativa ou desativa o suporte aos curingas "*", para indicar qualquer texto
    /// na posição, e "?", para indicar qualquer caracter na posição.
    /// </summary>
    public bool? AllowWildcards { get; set; }

    /// <summary>
    /// Ativa ou desativa a edição em múltiplas linhas, geralmente para campos texto.
    /// </summary>
    public bool? Multiline { get; set; }

    /// <summary>
    /// Opções de seleção do valor. Opcional.
    /// </summary>
    public Options<string> Options { get; set; }

    /// <summary>
    /// Utilitário para formatação de opção quando um título não é especificado.
    /// </summary>
    /// <param name="value">O valor da opção.</param>
    /// <returns>O valor formatado.</returns>
    public string FormatOption(string value) => value?.ToString() ?? "(Nenhum)";
  }
}