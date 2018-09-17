using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface IContext
  {
    IProvider Provider { get; }

    ICatalog Catalog { get; }
  }
}