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
  [ExposeQuery("/Consulta/Lacunas")]
  public class LacunasQuery
  {
    public class Filters
    {
      public IntWidget Serie { get; }
        = new IntWidget(nameof(Serie)) { Title = "Série" };

      public IntWidget Numero { get; }
        = new IntWidget(nameof(Numero)) { Title = "Número" };

      public TextWidget Chave { get; }
        = new TextWidget(nameof(Chave)) { Title = "Chave" };
    }

    public Filters Filter { get; } = new Filters();

    public Pagination Pagination { get; } = new Pagination { Limit = 20 };

    public string GetTitle() => "Lacunas no Período";

    public Links GetLinks()
      => new Links()
        .AddQuery<MenuQuery>("Menu");

    public Links GetRowLinks(Lacunas.Row row)
      => new Links()
        .AddSelfQuery<LacunaQuery>(q =>
        {
          q.Serie = row.Serie;
          q.Numero = row.Numero;
        });

    public IEnumerable<Lacunas.Row> GetRows()
    {
      var page =
        from row in Lacunas.Rows
        where (Filter.Numero.Value == null || (object)row.Numero == Filter.Numero.Value)
        where (Filter.Serie.Value == null || (object)row.Serie == Filter.Serie.Value)
        where (Filter.Chave.Value == null || (object)row.Chave == Filter.Chave.Value)
        select row;

      page = page.Skip(Pagination.Offset).Take(Pagination.Limit);
      return page;
    }
  }
}