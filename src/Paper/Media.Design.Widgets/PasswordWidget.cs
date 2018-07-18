using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets
{
  public class PasswordWidget : Widget
  {
    public PasswordWidget(string name)
      : base(name, DataTypeNames.Text, FieldTypeNames.Password)
    {
    }

    /// <summary>
    /// Valor do campo. Opcional.
    /// </summary>
    public new string Value
    {
      get => base.Value as string;
      set => base.Value = value;
    }
  }
}