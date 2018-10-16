using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Routing
{
  internal class LocalCatalog : ICatalog 
  {
    public LocalCatalog()
    {
    }

    public PaperBlueprint GetPaperBlueprint(string route)
    {
      throw new NotImplementedException();
    }
  }
}