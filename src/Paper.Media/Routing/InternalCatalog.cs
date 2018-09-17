using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  internal class InternalCatalog : ICatalog 
  {
    public InternalCatalog()
    {
    }

    public IPaper GetPaper(string route)
    {
      throw new NotImplementedException();
    }
  }
}