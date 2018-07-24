using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldHiddenAttribute : Attribute
  {
    public bool Hidden { get; }

    public FieldHiddenAttribute(bool allow = true)
    {
      Hidden = allow;
    }
  }
}