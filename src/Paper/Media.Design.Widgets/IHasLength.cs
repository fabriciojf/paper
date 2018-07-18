using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public interface IHasLength
  {
    /// <summary>
    /// Tamanho mínimo para um texto ou menor valor para um número.
    /// </summary>
    int? MinLength { get; set; }

    /// <summary>
    /// Tamanho máximo para um texto ou maior valor para um número.
    /// </summary>
    int? MaxLength { get; set; }
  }
}
