using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldReadonlyAttribute : Attribute
  {
    public bool ReadOnly { get; }

    public FieldReadonlyAttribute(bool allow = true)
    {
      ReadOnly = allow;
    }
  }
}