using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Queries;
using Paper.Media.Design.Sql;
using Paper.Media.Design.Widgets;
using Paper.Media.Design.Widgets.Mapping;
using Paper.Media.Rendering.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Toolset.Reflection;

namespace Paper.WebApp.Server.Demo.Consultas
{
  [ExposeQuery("/Consulta/Lacunas/{Serie}/{Numero}")]
  public class LacunaQuery
  {
    public int? Serie { get; set; }
    
    public int? Numero { get; set; }

    public string GetTitle() => "Lacuna";

    public Links GetLinks()
      => new Links()
        .AddQuery<MenuQuery>("Menu")
        .AddQuery<LacunasQuery>("Lacunas no Período");

    public Lacunas.Row GetData()
    {
      var data = (
        from row in Lacunas.Rows
        where row.Serie == this.Serie
        where row.Numero == this.Numero
        select row
      ).FirstOrDefault();
      return data;
    }
  }
}