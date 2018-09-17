using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public static class Provider
  {
    private static InternalProvider defaultProvider;

    public static IProvider GetDefaultProvider()
    {
      return defaultProvider ?? (defaultProvider = new InternalProvider());
    }
  }
}