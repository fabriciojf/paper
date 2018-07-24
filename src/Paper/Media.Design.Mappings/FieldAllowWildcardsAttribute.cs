using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldAllowWildcardsAttribute : Attribute
  {
    public bool AllowWildcards { get; }

    public FieldAllowWildcardsAttribute(bool allow = true)
    {
      AllowWildcards = allow;
    }
  }
}