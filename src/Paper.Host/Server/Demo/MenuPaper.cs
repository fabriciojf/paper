using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Papers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Toolset;
using Toolset.Reflection;

namespace Paper.WebApp.Server.Demo
{
  [Paper("/Menu")]
  public class MenuPaper : IPaperInfo, IPaperRows<ILink>
  {
    public Page RowsPage { get; }
    public Sort RowsSort { get; }
    public IFilter RowFilter { get; }

    public string GetTitle()
      => "Menu";

    public NameCollection GetClass() 
      => "index";

    public NameCollection GetRel()
      => null;

    public object GetProperties()
      => null;

    public IEnumerable<ILink> GetLinks()
      => null;

    public IEnumerable<ILink> GetRows()
      => new[]
      {
        new LinkSelf{ Title = "Blueprint", Href = "/Blueprint" },
        new LinkSelf{ Title = "Google.com", Href = "google.com" },
        new LinkSelf{ Title = "Menu", Href = "/Menu" }
      };

    public IEnumerable<HeaderInfo> GetRowHeaders(IEnumerable<ILink> rows)
      => new[]
      {
        new HeaderInfo { Name = "Title", Title = "Título" },
        new HeaderInfo { Name = "Href", Title = "Rota" }
      };

    public IEnumerable<ILink> GetRowLinks(ILink row)
      => new[] { row };
  }
}