using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Data
{
  public interface IVar
  {
    Type VarType { get; }

    VarKinds Kind { get; }

    bool IsNull { get; }

    object RawValue { get; set; }

    object Value { get; set; }

    bool HasWildcards { get; }

    string TextPattern { get; }

    IList List { get; set; }

    IEnumerable Map { get; set; }

    Range Range { get; set; }
  }
}