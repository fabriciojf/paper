using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Collections
{
  public class HashMap<T> : Map<string, T>
  {
    public HashMap()
    {
    }

    public HashMap(int capacity)
      : base(capacity)
    {
    }

    public HashMap(IEnumerable<KeyValuePair<string, T>> entries)
      : base(entries)
    {
    }
  }
}