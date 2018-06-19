using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Toolset.Collections;
using Toolset;

namespace Paper.Media
{
  public static class LinkCollectionExtensions
  {
    public static LinkCollection AddSelf(this LinkCollection links, string href)
    {
      var link = new Link { Rel = KnownRelations.Self, Href = href };
      links.Insert(0, link);
      return links;
    }

    public static LinkCollection AddSelf(this LinkCollection links, Uri href)
    {
      var link = new Link { Rel = KnownRelations.Self, Href = href.ToString() };
      links.Insert(0, link);
      return links;
    }
  }
}