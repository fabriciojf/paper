using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class PaperAttribute : Attribute
  {
    public PaperAttribute()
    {
    }

    public PaperAttribute(string route)
    {
      this.Route = route;
    }

    public string Route { get; set; }
  }
}