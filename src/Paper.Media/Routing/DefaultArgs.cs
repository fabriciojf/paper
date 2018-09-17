using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public class DefaultArgs : IArgs
  {
    public DefaultArgs(string templateUri, string requestUri)
    {
    }

    public object this[int index]
    {
      get => null;
      set { }
    }

    public object this[string name]
    {
      get => null;
      set { }
    }
  }
}