using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldMaxLengthAttribute : Attribute
  {
    public int MaxLength { get; }

    public FieldMaxLengthAttribute(int minLength)
    {
      MaxLength = minLength;
    }
  }
}