using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public class BitWidget : Widget
  {
    public BitWidget(string name)
      : base(name, KnownFieldDataTypes.Bit)
    {
    }

    /// <summary>
    /// Valor do campo. Opcional.
    /// </summary>
    public new bool? Value
    {
      get => base.Value as bool?;
      set => base.Value = value;
    }
  }
}