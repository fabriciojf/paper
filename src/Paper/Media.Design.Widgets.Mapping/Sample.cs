using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Widgets.Mapping
{
  class Sample
  {
    [Widget]
    public int? Id { get; set; }

    [TextWidget]
    public string Status { get; set; }

    [WidgetOptions(For = "Status")]
    public Option<string>[] StatusOptions => null;

    [TextWidget]
    public int EmpresaId { get; set; }

    [WidgetOptions(
      For = "EmpresaId",
      ValueKey = "DFcod_empresa",
      TitleKey = "DFnome_fantasia"
    )]
    public object[] GetEmpresas() => null;
  }
}