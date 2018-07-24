using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldRequiredAttribute : Attribute
  {
    public bool Required { get; }

    public FieldRequiredAttribute(bool allow = true)
    {
      Required = allow;
    }
  }
}