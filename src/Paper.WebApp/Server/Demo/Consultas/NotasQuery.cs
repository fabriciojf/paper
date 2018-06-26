using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Queries;
using Paper.Media.Design.Sql;
using Paper.Media.Rendering.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Toolset.Reflection;

namespace Paper.WebApp.Server.Demo.Consultas
{
  [ExposeQuery("/Consulta/Notas")]
  public class NotasQuery : IRowsQuery<object, object>, IHasQueryMetadata
  {
    public object Filter => null;

    public Sort Sort => null;

    public Pagination Pagination => null;

    public NameCollection GetClass() => "index";

    public NameCollection GetRels() => null;

    public object GetProperties() => null;

    public string GetTitle() => "Notas Fiscais no Período";

    public Links GetLinks() => null;
      //=> new Links()
      //  .Add("http://google.com", "Google.com");

    public Links GetRowLinks(object row)
      => new Links()
        .AddSelf(
          href: row.Get<string>("_Link"),
          title: row.Get<string>("Module")
        );

    public Cols GetRowsHeaders()
      => new Cols()
        .Add("Module", "Módulo")
        .Add("Product", "Produto")
        .Add("Description", "Descrição")
        .Add("Version", "Versão")
        .Add("Date", "Data")
        .AddHidden("_Link");

    public IEnumerable<object> GetRows()
    {
      yield return new
      {
        Module = "Rejeições no Período",
        Description = "Notas fiscais rejeitadas ou não-transmitidas no período.",
        _Link = "{Api}/Consulta/Rejeicoes"
      };
      yield return new
      {
        Module = "Notas Fiscais no Período",
        Description = "Coleção das notas fiscais no período.",
        _Link = "{Api}/Consulta/Notas"
      };
      yield return new
      {
        Module = "Inutilizações no Período",
        Description = "Rastreio dos números de nota não utilzados no período.",
        _Link = "{Api}/Consulta/Inutilizacao"
      };
    }
  }
}