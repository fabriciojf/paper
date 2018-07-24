using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldCategoryAttribute : Attribute
  {
    public string Category { get; }

    public FieldCategoryAttribute(string category)
    {
      Category = category;
    }
  }
}