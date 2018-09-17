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

namespace Paper.Host.Server.Demo
{
  [Expose, Paper("/Menu")]
  public class MenuPaper : IPaperBasics, IPaperRows<IFilter, ILink>
  {
    public Page Page { get; }
    public Sort Sort { get; }
    public IFilter Filter { get; }

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
      => new ILink[]
      {
        new LinkSelf{ Title = "Blueprint", Href = "/Blueprint" },
        new LinkSelf{ Title = "Google.com", Href = "google.com" },
        new LinkSelf{ Title = "Menu", Href = "/Menu" },
        new LinkTo{ Title = "SampleEntity", Href = "/Paper/Host/Server/Demo/Sample" },
        new LinkTo{ Title = "Users", Href = "/Paper/Host/Server/Demo/Users" }
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