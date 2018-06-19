using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public class TimeWidget : Widget, IHasMany, IHasRange, IHasOptions<TimeSpan>
  {
    public TimeWidget(string name)
      : base(name, KnownFieldDataTypes.Time)
    {
    }

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
    public Options<TimeSpan> Options { get; set; }

    /// <summary>
    /// Utilitário para formatação de opção quando um título não é especificado.
    /// </summary>
    /// <param name="value">O valor da opção.</param>
    /// <returns>O valor formatado.</returns>
    public string FormatOption(TimeSpan value) => value.ToString("HH:mm:ss");
  }
}