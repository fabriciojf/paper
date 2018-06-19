using System;

namespace Paper.Media.Design.Queries
{
  public static class QueryLinkExtensions
  {
    public static Links AddQuery<T>(
        this Links links
      , Action<T> args = null
      , string title = null
      , NameCollection rel = null
      , NameCollection classes = null
      )
    {
      var link = new QueryLink
      {
        QueryType = typeof(T),
        Rel = rel,
        Title = title,
        Class = classes
      };

      if (args != null)
      {
        link.Setter = query => args.Invoke((T)query);
      }

      links.Add(link);
      return links;
    }

    public static Links AddQuery<T>(
        this Links links
      , string title
      , NameCollection rel = null
      , NameCollection classes = null
      )
    {
      return AddQuery<T>(links, null, title, rel, classes);
    }

    public static Links AddSelfQuery<T>(
        this Links links
      , Action<T> args = null
      , string title = null
      , NameCollection rel = null
      , NameCollection classes = null
      )
    {
      (rel ?? (rel = new NameCollection())).Add(KnownRelations.Self);
      return AddQuery<T>(links, args, title, rel, classes);
    }

    public static Links AddSelfQuery<T>(
        this Links links
      , string title = null
      , NameCollection rel = null
      , NameCollection classes = null
      )
    {
      (rel ?? (rel = new NameCollection())).Add(KnownRelations.Self);
      return AddQuery<T>(links, null, title, rel, classes);
    }
  }
}
