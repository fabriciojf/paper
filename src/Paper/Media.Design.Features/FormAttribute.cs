using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Features
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
  public class FormAttribute : Attribute
  {
    public FormAttribute(Type type) { }
  }
}
