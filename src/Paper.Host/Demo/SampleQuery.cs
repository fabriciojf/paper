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

namespace Paper.WebApp.Host.Demo
{
  [ExposeQuery("/Menu")]
  public class SampleQuery : IRowsQuery<object, object>, IHasQueryMetadata
  {
    public object Filter => null;

    public Sort Sort => null;

    public Pagination Pagination => null;

    public NameCollection GetClass() => "index";

    public NameCollection GetRels() => null;

    public object GetProperties() => null;

    public string GetTitle() => $"Menu";

    public Links GetLinks()
      => new Links()
        .Add("http://google.com", "Google.com");

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
        Module = "Este Menu",
        Description = "Menu de demonstração do Paper",
        _Link = "{Api}/Menu"
      };
      yield return new
      {
        Module = "Exemplo de Entidade",
        Description = "Exemplo de entidade renderizada diretamente",
        _Link = "{Api}/Sample"
      };
      yield return new
      {
        Module = "Google.com",
        Description = "Site de buscas do Google",
        _Link = "http://google.com"
      };
    }
  }
}