using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolset;

namespace Media.Service.Utilities
{
  public class PathIndex : PathIndex<string>
  {
    public void Add(string path)
    {
      base.Add(path, path);
    }
  }
}