using System;
using System.Collections.Generic;
using System.Text;

namespace Inkeeper.RestApi
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RestAttribute : Attribute
  {
    public string Route { get; set; }

    public RestAttribute(string route)
    {
      this.Route = route;
    }
  }
}
