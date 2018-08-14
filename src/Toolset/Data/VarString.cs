using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;

namespace Toolset.Data
{
  public class VarString : VarAny<string>
  {
    public bool HasWildcards => ((IVar)this).HasWildcards;

    public string TextPattern => ((IVar)this).TextPattern;

    public static implicit operator VarString(string text)
    {
      return new VarString { Value = text };
    }

    public static implicit operator VarString(List<string> list)
    {
      return new VarString { List = list };
    }

    public static implicit operator VarString(string[] list)
    {
      return new VarString { List = list };
    }

    public static implicit operator VarString(Dictionary<string, string> map)
    {
      return new VarString { Map = map };
    }

    public static implicit operator VarString(Map<string, string> map)
    {
      return new VarString { Map = map };
    }

    public static implicit operator VarString(RangeEx<string> range)
    {
      return new VarString { Range = range };
    }
  }
}
