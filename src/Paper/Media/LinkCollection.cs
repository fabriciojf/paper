using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Paper.Media
{
  /// <summary>
  /// Coleção de links.
  /// </summary>
  [CollectionDataContract(Namespace = Namespaces.Default, Name = "Links")]
  public class LinkCollection : List<Link>
  {
    public LinkCollection()
    {
    }

    public LinkCollection(IEnumerable<Link> items)
    : base(items)
    {
    }

    [Obsolete]
    [Browsable(false)]
    public Link Add(string rel, string href)
    {
      var link = new Link { Rel = rel, Href = href };
      Add(link);
      return link;
    }

    [Obsolete]
    [Browsable(false)]
    public Link Add(string rel, string title, string href)
    {
      var link = new Link { Rel = rel, Title = title, Href = href };
      Add(link);
      return link;
    }
  }
}