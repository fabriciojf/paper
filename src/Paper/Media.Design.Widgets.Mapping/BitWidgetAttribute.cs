using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets.Mapping
{
  public class BitWidgetAttribute : WidgetAttribute
  {
    public BitWidgetAttribute()
      : base(KnownFieldDataTypes.Bit)
    {
    }
  }
}