using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Graph
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  internal class GraphIgnoreAttribute : Attribute
  {
  }
}
