using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public interface IHasRange
  {
    /// <summary>
    /// Ativa ou desativa o suporte a intervalo, na forma "{ min=x, max=y }".
    /// </summary>
    bool? AllowRange { get; set; }
  }
}
