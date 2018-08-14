using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Structures
{
  public interface IVar
  {
    Type VarType { get; }

    VarKind Kind { get; }

    object RawValue { get; set; }

    object Value { get; set; }

    bool HasWildcards { get; }

    string TextPattern { get; }

    IEnumerable List { get; set; }

    IEnumerable Map { get; set; }

    Range Range { get; set; }
  }
}