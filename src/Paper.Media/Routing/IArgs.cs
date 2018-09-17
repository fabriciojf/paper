using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface IArgs
  {
    object this[int index] { get; set; }

    object this[string name] { get; set; }
  }
}