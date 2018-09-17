using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public class Route
  {
    private readonly string uri;

    private Route(string uri)
    {
      this.uri = uri;
    }

    public string Path { get; set; }

    public string PathAndQueryString { get; set; }

    public override string ToString()
    {
      return uri;
    }

    public static implicit operator Route(string route)
    {
      return new Route(route);
    }

    public static implicit operator string(Route route)
    {
      return route.uri;
    }
  }
}