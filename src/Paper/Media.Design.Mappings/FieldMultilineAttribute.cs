using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldMultilineAttribute : Attribute
  {
    public bool Multiline { get; }

    public FieldMultilineAttribute(bool allow = true)
    {
      Multiline = allow;
    }
  }
}