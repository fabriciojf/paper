using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public class DecimalWidget : Widget, IHasLength, IHasMany, IHasRange, IHasOptions<decimal>
  {
    public DecimalWidget(string name)
      : base(name, KnownFieldDataTypes.Decimal)
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
    /// Ativa ou desativa a múltipla seleção de valores para o campo.
    /// </summary>
    public bool? AllowMany { get; set; }

    /// <summary>
    /// Ativa ou desativa o suporte a intervalo, na forma "{ min=x, max=y }".
    /// </summary>
    public bool? AllowRange { get; set; }

    /// <summary>
    /// Opções de seleção do valor. Opcional.
    /// </summary>
    public Options<decimal> Options { get; set; }

    /// <summary>
    /// Utilitário para formatação de opção quando um título não é especificado.
    /// </summary>
    /// <param name="value">O valor da opção.</param>
    /// <returns>O valor formatado.</returns>
    public string FormatOption(decimal value) => value.ToString("#,##0.00");
  }
}