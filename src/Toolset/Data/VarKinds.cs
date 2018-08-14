using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Data
{
  [Flags]
  public enum VarKinds
  {
    Null = 0,
    Value = 1,

    Primitive = Value | 2,
    Graph = Value | 4,
    Text = Value | 8,

    List = 16,
    Map = 32,
    Range = 64,
  }
}