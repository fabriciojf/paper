using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public class AggregateCatalog : ICatalog
  {
    public void Add(ICatalog catalog)
    {
      // ...
    }

    public IPaper GetPaper(string route)
    {
      throw new NotImplementedException();
    }
  }
}