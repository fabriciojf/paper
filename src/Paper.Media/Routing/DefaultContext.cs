using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public class DefaultContext : IContext
  {
    public IProvider Provider { get; set; }
    public ICatalog Catalog { get; set; }
  }
}