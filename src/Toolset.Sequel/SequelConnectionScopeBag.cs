using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  class SequelConnectionScopeBag
  {
    private readonly Dictionary<string, SequelConnectionScope> bag = new Dictionary<string, SequelConnectionScope>(1);
    private Stack<SequelConnectionScope> history = new Stack<SequelConnectionScope>();

    private const string DefaultScopeName = "__internal__";

    public SequelConnectionScope this[string name]
    {
      get
      {
        name = name ?? DefaultScopeName;
        return bag.ContainsKey(name) ? bag[name] : null;
      }
      set
      {
        name = name ?? DefaultScopeName;
        if (value == null)
        {
          if (bag.ContainsKey(name))
            bag.Remove(name);
        }
        else
        {
          bag[name] = value;
        }
      }
    }
  }
}
