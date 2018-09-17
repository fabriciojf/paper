using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface ICatalog
  {
    IPaper GetPaper(string route);
  }
}