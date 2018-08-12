using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Collections
{
  public class HashMap : Map<string, object>
  {
    public HashMap()
    {
    }

    public HashMap(int capacity)
      : base(capacity)
    {
    }

    public HashMap(IEnumerable<KeyValuePair<string, object>> entries)
      : base(entries)
    {
    }
  }
}