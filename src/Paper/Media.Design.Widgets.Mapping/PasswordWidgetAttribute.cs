using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets.Mapping
{
  public class PasswordWidgetAttribute : WidgetAttribute
  {
    public PasswordWidgetAttribute()
      : base(KnownFieldDataTypes.Text, KnownFieldTypes.Password)
    {
    }
  }
}