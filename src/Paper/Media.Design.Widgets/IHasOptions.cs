using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public interface IHasOptions<TOption>
  {
    /// <summary>
    /// Opções de seleção do valor. Opcional.
    /// </summary>
    Options<TOption> Options { get; set; }

    /// <summary>
    /// Utilitário para formatação de opção quando um título não é especificado.
    /// </summary>
    /// <param name="value">O valor da opção.</param>
    /// <returns>O valor formatado.</returns>
    string FormatOption(TOption value);
  }
}
