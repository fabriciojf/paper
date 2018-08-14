using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;

namespace Toolset.Data
{
  public class VarAny : VarAny<object>
  {
    public VarAny()
    {
    }

    public VarAny(object rawValue)
    {
      this.RawValue = rawValue;
    }

    public bool HasWildcards => ((IVar)this).HasWildcards;

    public string TextPattern => ((IVar)this).TextPattern;
  }
}
