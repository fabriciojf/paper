using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design
{
  public static class HrefLinkExtensions
  {
    public static Links Add(
        this Links links
      , string href
      , string title = null
      , NameCollection rel = null
      , string mediaType = null
      , NameCollection classes = null
      , object[] args = null
      )
    {
      links.Add(new HrefLink
      {
        Href = href,
        Title = title,
        Rel = rel,
        Type = mediaType,
        Class = classes
      });
      return links;
    }

    public static Links AddSelf(
        this Links links
      , string href
      , string title = null
      , NameCollection rel = null
      , string mediaType = null
      , NameCollection classes = null
      , object[] args = null
      )
    {
      (rel ?? (rel = new NameCollection())).Add(KnownRelations.Self);
      links.Add(new HrefLink
      {
        Href = href,
        Title = title,
        Rel = rel,
        Type = mediaType,
        Class = classes
      });
      return links;
    }
  }
}
